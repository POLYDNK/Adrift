using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Stat
{
    public int baseValue, min, max;
    public StatType type;

    //Return value adjusted for modifiers
    public int GetValue(List<StatModifier> modifiers)
    {
        float value = baseValue;
        //Add flat values first
        foreach(StatModifier modifier in modifiers)
        {
            if(modifier.type != type || modifier.op == OpType.Multiply) continue;
            value += modifier.value;
        }
        float scalar = 1;
        //Combine into one large scalar for multiplication
        foreach(StatModifier modifier in modifiers)
        {
            if(modifier.type != type || modifier.op == OpType.Add) continue;
            scalar += modifier.value;
        }
        //Multiply by scalar
        value *= scalar;
        return (int) Mathf.Round(Mathf.Clamp(value, min, max));
    }
}

[Serializable]
public class StatModifier
{
    public StatType type; //Target stat
    public OpType op; //Whether to do addition or multiplication
    public int duration; //Duration in turns (-1 for infinite)
    public float value; //Value for operation
    public float chance; //Chance to apply (0.0 to 1.0)
    public string id; //Optional ID to identify this modifier after it's applied
    public Sprite statusIcon;

    public StatModifier(StatType type, OpType op, int duration, float value, float chance, string id = "", Sprite statusIcon = null) {
        this.type = type;
        this.op = op;
        this.duration = duration;
        this.value = value;
        this.chance = chance;
        this.id = id;
        this.statusIcon = statusIcon;
    }

    public StatModifier(StatType type, OpType op, int duration, float value, float chance, string id) : this(type, op, duration, value, chance) {
        this.id = id;
    }

    //Return true if this modifier is a buff, false if a debuff
    public bool IsBuff() {
        if(op == OpType.Add) return value >= 0;
        else return value >= 1;
    }

    public StatModifier Clone() {
        return new StatModifier(type, op, duration, value, chance, id, statusIcon);
    }
}

public enum StatType 
{
    Hpmax, Apmax, Str, Def, Spd, Dex, Lck, Mv
}

public enum OpType
{
    Add, Multiply
}

public enum ClassName 
{
    Pirate, Bard, Mercenary, Merchant, Buccaneer, Acolyte, PirateCaptain, MerchantCaptain, MonarchyCaptain
}

public class Character : MonoBehaviour
{
    public static readonly string WeaponModel = "Weapon.R";
    [FormerlySerializedAs("HPMAX")] [SerializeField]
    public Stat hpmax; //Maximum health
    [FormerlySerializedAs("APMAX")] public Stat apmax; //Maximum ability points, default value is 3
    [FormerlySerializedAs("STR")] public Stat str; //Strength
    [FormerlySerializedAs("DEF")] public Stat def; //Defense
    [FormerlySerializedAs("SPD")] public Stat spd; //Agility
    [FormerlySerializedAs("DEX")] public Stat dex; //Dexterity
    [FormerlySerializedAs("LCK")] public Stat lck; //Luck
    [FormerlySerializedAs("MV")] public Stat mv; //Movement

    [FormerlySerializedAs("HP")] public int hp; //Current health
    [FormerlySerializedAs("AP")] public int ap; //Current ability points
    /*
    public int MoraleMAX; //Maximum Morale
    public int MoraleMIN; //Minimum Morale 
    */
    [FormerlySerializedAs("ATK")] public int atk; //Attack power (= STR - Enemy's DEF)
    [FormerlySerializedAs("HIT")] public int hit; //Hit Rate (= (((DEX*3 + LCK) / 2) - Enemy's AVO)
    [FormerlySerializedAs("CRIT")] public int crit; //Critical Rate (= ((DEX / 2) - 5) - Enemy's LCK)
    [FormerlySerializedAs("AVO")] public int avo; //Avoid  (= (SPD*3 + LCK) / 2)



    //Status Effects (Resist)
    [FormerlySerializedAs("charmRES")] public bool charmRes = false;
    [FormerlySerializedAs("tauntRES")] public bool tauntRes = false;
    [FormerlySerializedAs("dmGimmune")] [FormerlySerializedAs("DMGimmune")] public bool dmgImmune = false;

    public List<StatModifier> statModifiers = new List<StatModifier>();

    [FormerlySerializedAs("name")] [FormerlySerializedAs("Name")] public string displayName;
    [FormerlySerializedAs("classname")] public ClassName className;

    // Actions
    public Ability basicAttack, comboAttack; //Generic actions
    public List<Ability> abilities; //Unique abilities

    // Healthbar
    [SerializeField] public float healthBarScale;
    public RectangleBar healthBar;
    public float healthBarYOffset;

    // AP Bar
    [FormerlySerializedAs("APBarScale")] [SerializeField] public float apBarScale;
    [FormerlySerializedAs("APBar")] public IconBar apBar;
    [FormerlySerializedAs("APBarYOffset")] public float apBarYOffset;

    // Canvas reference
    public GameObject canvas;

    // UI
    [FormerlySerializedAs("DamageText")] public GameObject damageText;

    // Model reference
    public GameObject model;
    public Sprite icon;

    // Character's logical position on the grid
    public Vector2Int gridPosition;
    public GameObject myGrid;

    // Crew that this character belongs to (should be set by CrewSystem, contains morale value)
    public GameObject crew;

    //Equipment
    public Weapon weapon; //increases ATK and can modify max HP, DEF, SPD, DEX, and LCK (either melee or ranged)
    public Armor armor; //increases DEF
    public Hat hat; //can increase STR, DEF, SPD, and/or DEX
    public Ring ring; //can increase ATK, HIT, CRIT, and/or AVO
    public Amulet amulet; //can increase max HP, LCK, and/or healing ability
    public Bracelet bracelet; //can increase max AP, LCK, and/or healing ability
    public Shoes shoes; //can increase MV and/or SPD
    public Aura aura; //can increase any stat

    [FormerlySerializedAs("Morale")] public int morale;

    // Start is called before the first frame update
    void Start()
    {
        // Attach healthbar to canvas
        canvas = GameObject.Find("UI Overlay");
        healthBar = Instantiate(healthBar, canvas.transform);
        apBar = Instantiate(apBar, canvas.transform);

        // Set gradient color
        bool isPlayer = crew.GetComponent<Crew>().isPlayer;
        healthBar.gradient = HealthBarGradient(isPlayer);

        // Morale is now pulled from CrewSystem.morale. All characters now use a univeral morale value, with the same boosts
        morale = crew.GetComponent<Crew>().morale; //Morale (from 0 - 100), (from crew)
        //depending on this value, ATK/CRIT are boosted from +1 to +5 , HIT/AVO is boosted by +2 to +10 , and Healing is boosted by +1 to +10

        //MoraleMIN = 0;

        //Temporary name for player-controlled characters for logging: Will need to have functionality added later
        displayName = "Pupperton";

        // Set health, ability points, etc.

        Refresh();

        hp = GetMaxHp();
        healthBar.SetMaxHealth(hp);
        healthBar.SetHealth(hp);

        //Morale = MoraleMAX;

        ap = GetMaxAP();
        apBar.SetMaxValue(ap);
        apBar.SetValue(ap);

        UpdateAvo();
        hit = GetHit(null, true);
        crit = GetCrit(null, true);
        atk = GetAtk(null, true);





    }

    // Get a gradient for healthbar colors
    public static Gradient HealthBarGradient(bool isFriendy)
    {
        Gradient grad = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = isFriendy ? Color.yellow : new Color(255f/255f, 165f/255f, 0f, 1f);
        colorKey[0].time = 0.0f;
        colorKey[1].color = isFriendy ? Color.green : Color.red;
        colorKey[1].time = 1.0f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 0.5f;

        grad.SetKeys(colorKey, alphaKey);

        return grad;
    }

    // Update is called once per frame
    void Update()
    {
        // Health Testing
        // if (Input.GetKeyDown(KeyCode.Space))
        // {

        //     //damage
        //     adjustHP(-20, false);
        // }

        //updateAVO(ring, aura);
        //update equipment

    }

    // Update bar positions
    void LateUpdate()
    {
        // --------- Update bar positions ---------
        healthBar.transform.position = Camera.main.WorldToScreenPoint(model.transform.position + new Vector3(0, healthBarYOffset, 0));
        apBar.transform.position = Camera.main.WorldToScreenPoint(model.transform.position + new Vector3(-0.375f, apBarYOffset, 0.2f));

        // --------- Update scale based on camera position ---------
        float camDist = Vector3.Distance(Camera.main.transform.position, this.transform.position);
        float newScale = 0.0f, apScale = 0.0f;

        if (camDist != 0.0f)
        {
            newScale = healthBarScale / camDist;
            apScale = apBarScale / camDist;
        }

        healthBar.transform.localScale = new Vector3(newScale, newScale, newScale);
        apBar.transform.localScale = new Vector3(apScale, apScale, apScale);
    }

    // Disable the character and associated elements
    public void RemoveFromGrid() {
        this.gameObject.SetActive(false);
        HideBars();
    }

    public GameObject GetTileObject() {
        return myGrid.GetComponent<Grid>().GetTileAtPos(gridPosition);
    }

    // --------------------------------------------------------------
    // @desc: Move this character along an automatically generated path to its destination
    // @arg: destTile   - grid tile to move the character to
    // @ret: bool      - whether the move is successful or not
    // --------------------------------------------------------------
    public bool PathToTile(Tile destTile, bool onlyHighlighted, bool simulate = false)
    {
        bool moveSuccess = false;
        
        // Get tile on source position
        GameObject sourceTile = GetTileObject();
        var sourceTileScript = sourceTile.GetComponent<Tile>();

        // Get tile on dest position
        var destTileScript = destTile.GetComponent<Tile>();
        var destPos = destTileScript.position;

        // Get character on source tile
        if (sourceTileScript.hasCharacter && !destTileScript.hasCharacter && destTileScript.passable)
        {
            // Only move to highlighted tiles
            if (!onlyHighlighted || destTileScript.highlighted)
            {
                if (simulate) return true;
                Debug.Log("Moving character to tile " + destPos.x + " " + destPos.y);

                // Move character to destPos
                destTileScript.PathRef.PathToRootOnStack(GetComponent<FollowPath>().PathToFollow);

                // Move camera to destPos
                Camera.main.GetComponent<CameraController>().SetCameraFollow(this.gameObject);

                // Set source tile data
                sourceTileScript.hasCharacter = false;
                sourceTileScript.characterOn = null;

                // Set destination tile data
                destTileScript.hasCharacter = true;
                destTileScript.characterOn = this.gameObject;

                myGrid = destTile.grid.gameObject;
                gridPosition = destPos;
                transform.SetParent(myGrid.transform.parent.transform, true); // Parent this character to the new ship

                moveSuccess = true;
            }
        }
        else
        {
            Debug.Log("PathCharacterOnTile: Error! source tile does not have a character");
        }

        return moveSuccess;
    }

    // Rotate the character around the y-axis to face the target position over time, return true if already facing
    public bool RotateTowards(Vector3 targetPos) {
        // This logic is extremely cursed but it works
        Vector2 xzDist = new Vector2(targetPos.x - transform.position.x, transform.position.z - targetPos.z);
        float angle = Mathf.Atan2(xzDist.y, xzDist.x) * Mathf.Rad2Deg;
        Quaternion q0 = Quaternion.AngleAxis(angle, Vector3.up);
        Quaternion q1 = Quaternion.Euler(q0.eulerAngles.x, q0.eulerAngles.y + 90, q0.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, q1, 12.0f * Time.deltaTime);
        return Mathf.Abs(q1.eulerAngles.y - transform.eulerAngles.y) < 1f;
    }

    // Activate bars
    public void ShowBars() {
        healthBar.gameObject.SetActive(true);
        apBar.gameObject.SetActive(true);
    }

    // Deactivate bars
    public void HideBars() {
        healthBar.gameObject.SetActive(false);
        apBar.gameObject.SetActive(false);
    }

    // Whether this character is considered dead in battle
    public bool IsDead() {
        return hp <= 0;
    }

    public bool IsFullHealth() {
        return hp == GetMaxHp();
    }

    public bool IsPlayer() {
        return crew.GetComponent<Crew>().isPlayer;
    }

    // Return all usable abilities in battle
    public List<Ability> GetBattleAbilities() {
        List<Ability> list = new List<Ability>();
        list.Add(basicAttack);
        foreach(Ability ability in abilities) list.Add(ability); //Class abilities
        if(weapon != null) {
            foreach(Ability ability in weapon.abilities) list.Add(ability); //Weapon abilities
        }
        return list;
    }

    // Clear and re-apply all permanent modifiers and reload equipment models
    public void Refresh() {
        ClearModifiers();
        if(weapon != null) {
            weapon.ApplyModifiers(this);
            //Remove the old model first
            Transform oldModel = DataUtil.RecursiveFind(this.transform, WeaponModel);
            if(oldModel != null) Destroy(oldModel.gameObject);
            //Instantiate the new weapon and parent it to the right hand's attachment point for objects
            GameObject newModel = Instantiate(weapon.model, DataUtil.RecursiveFind(this.transform, "Object.R"));
            newModel.transform.Rotate(-90f, 0f, 0f, Space.Self); //Set up rotations (this should be done in the prefab if more stuff is used here besides swords)
        }
        if(hat != null) hat.ApplyModifiers(this);
        if(ring != null) ring.ApplyModifiers(this);
        if(amulet != null) amulet.ApplyModifiers(this);
        if(bracelet != null) bracelet.ApplyModifiers(this);
        if(shoes != null) shoes.ApplyModifiers(this);
        if(aura != null) aura.ApplyModifiers(this);
        if(armor != null) armor.ApplyModifiers(this);

        UpdateAvo();
        hit = GetHit(null, true);
        crit = GetCrit(null, true);
        atk = GetAtk(null, true);
    }

    // HP changed (either taking damage (negative) or healing (positive))
    // if dontKill is true, lethal damage will only reduce HP to 1
    public void AdjustHp(int change, bool dontKill)
    {

        if(change < 0 && dmgImmune){

            change = 0;
        }

        hp += change;

        if(hp < 0 && !dontKill){
            hp = 0;
        }
        else if(hp <= 0 && dontKill){

            hp = 1;
        }

        if(hp > GetMaxHp()){
            hp = GetMaxHp();
        }

        // Display Damage
        var damageScript = damageText.GetComponent<TextDamage>();
        damageScript.damageToDisplay = Mathf.Abs(change);
        damageScript.objectFollowing = model;
        GameObject damageDisplay = Instantiate(damageText, canvas.transform);

        // Update Healthbar
        healthBar.SetHealth(hp);
    }

    public void AddAP(int change) {
        ap = Mathf.Clamp(ap + change, 0, GetMaxAP());
        apBar.SetValue(ap);
    }

    //AP changed (either positive or negative)
    //subType is primarily for subtractions (either ability used (1) or AP drained (2) [0 if not subtraction])
    //returns 0 if adjustment was successful, 1 otherwise
    public int AdjustAP(int change, int subType){

        int oldAP = ap;
        ap += change;

        if(subType != 0){

            if(ap < 0){

                if(subType == 1){

                    ap = oldAP; //not enough AP!
                    return 1;

                }
                else{

                    ap = 0;
                    return 0;

                }

            }

            return 0;
        }
        if(ap > GetMaxAP()){
            ap = GetMaxAP();
        }

        return 0;
    }

    public void TickModifiers() {
        List<StatModifier> removals = new List<StatModifier>();
        foreach(StatModifier modifier in statModifiers) {
            if(modifier.duration > -1) modifier.duration--;
            if(modifier.duration == 0) removals.Add(modifier);
        }
        foreach(StatModifier modifier in removals){

            statModifiers.Remove(modifier);
            if(modifier.id == "Immune"){
                dmgImmune = false;
            }

        } 
    }

    public void AddModifier(StatModifier modifier) {
        statModifiers.Add(modifier.Clone());
    }

    public void AddModifiers(List<StatModifier> modifiers) {
        foreach(StatModifier modifier in modifiers) {
            AddModifier(modifier);
        }
    }

    public void ClearModifiersWithId(string id) {
        foreach(StatModifier modifier in statModifiers) {
            if(modifier.id == id) statModifiers.Remove(modifier);
        }
    }

    public void ClearModifiers() {
        statModifiers.Clear();
    }

    public void ClearDebuffs(){
        List<StatModifier> debuffs = new List<StatModifier>();
        foreach(StatModifier modifier in statModifiers) {
            if(!modifier.IsBuff()) debuffs.Add(modifier);
        }
        foreach(StatModifier modifier in debuffs) statModifiers.Remove(modifier);

    }


    //Morale changed (either positive or negative)
    public void AdjustMorale(int change){
        morale += change;
        if(morale < 0){
            morale = 0;
        }
        if(morale > 100){
            morale = 100;
        }
    }


    //Attack the enemy, possibly with a critical hit
    //Note: Critical hits triple the total damage
    //Type 1 = general attack, nothing changes
    //Type 2 = no defenses, attack while target.DEF is ignored
    //Type 3 = attack doesn't miss
    //ATK/CRIT/HIT can be modified further with abilities/passives
    public int Attack(Character target, int type, int atkMod, int hitMod, int critMod){
        int dex = GetDexterity(), str = GetStrength(), lck = GetLuck();
        hit = ((((dex * 3 + lck) / 2) + (2 * (morale / 20))) - target.avo) + AccessoryBonus(1) + hitMod;
        crit = ((((dex / 2) - 5) + (morale / 20)) - target.GetLuck()) + AccessoryBonus(2) + critMod;

        if(weapon != null && weapon.lastStand && hp == 1){//Last Stand gives a gauranteed crit

            atk += ((str + (morale / 20) + WeaponBonus() + AccessoryBonus(0) + atkMod) - target.GetDefense()) * 3;
            return atk;
        }

        if(DetermineCrit(crit)){

            if(type == 2){

                atk = (str + (morale / 20) + WeaponBonus() + AccessoryBonus(0) + atkMod) * 3;
            }
            else{

                atk = ((str + (morale / 20) + WeaponBonus() + AccessoryBonus(0) + atkMod) - target.GetDefense()) * 3; //CRITICAL HIT!


            }

            if(weapon != null){

                weapon.WeaponDamage();

            }

        }
        else if(DetermineHit(hit) || type == 3){

            if(type == 2){

                atk = (str + (morale / 5) + WeaponBonus() + AccessoryBonus(0) + atkMod); //HIT!


            }
            else{

                atk = (str + (morale / 5) + WeaponBonus() + AccessoryBonus(0) + atkMod) - target.GetDefense(); //HIT!



            }

            if(weapon != null){

                weapon.WeaponDamage();

            }

        }
        else{

            atk = 0; //Miss...
            
        }


        return atk;
    }

    public bool DetermineHit(int hit){

       
        if(hit >= UnityEngine.Random.Range(0, 100)){
            return true;
        }
        else{
            return false;
        }
    }

    public bool DetermineCrit(int crit){

        
        if(crit >= UnityEngine.Random.Range(0,100)){
            return true;
        }
        else{
            return false;
        }
    }

    public int GetAtk(Character target, bool noDef){

        if(noDef){

            return (GetStrength() + (morale / 20) + WeaponBonus() + AccessoryBonus(0));
        }
        else{

            return (GetStrength() + (morale / 20) + WeaponBonus() + AccessoryBonus(0)) - target.GetDefense();


        }


    }

    public int GetHit(Character target, bool noAvo){

        if(noAvo){

            return ((((GetDexterity() * 3 + GetLuck()) / 2) + (2 * (morale / 20)))) + AccessoryBonus(1);
        }
        else{

            return ((((GetDexterity() * 3 + GetLuck()) / 2) + (2 * (morale / 20))) - target.avo) + AccessoryBonus(1);


        }


    }

    public int GetCrit(Character target, bool noLck){

        if(noLck){

            return ((((GetDexterity() / 2) - 5) + (morale / 20))) + AccessoryBonus(2);
        }
        else{

            return ((((GetDexterity() / 2) - 5) + (morale / 20)) - target.GetLuck()) + AccessoryBonus(2);


        }
        
    }

    public int WeaponBonus(){

        if(weapon != null && !(weapon.isBroken)){

            return weapon.mgt;

        }
        else{

            return 0;
        }
    }

    

    public int AccessoryBonus(int type){

        int totalBonus = 0;


        if(ring != null){

            totalBonus += ring.BattleBonus(type);
        }
        
        if(aura != null){

            totalBonus += aura.BattleBonus(type);
        }

        return totalBonus;

        
    }


    public void UpdateAvo(){

        int totalMod = 0;
        if(ring != null){

            totalMod += ring.avOmodifier;

        }

        if(aura != null){

            totalMod += aura.avOmodifier;
        }

        avo = ((GetSpeed()*3 + GetLuck()) / 2) + (2 * (morale / 20)) + totalMod;

    }

    public int Heal(Character target){

        int healBonus = 0;
        if(amulet != null){
            healBonus += amulet.healModifier;
        }

        if(bracelet != null){
            healBonus += bracelet.healModifier;
        }

        if(aura != null){

            healBonus += aura.healModifier;
        }

        healBonus += morale / 10;

        return healBonus;



    }

    public int GetMaxHp() {
        return hpmax.GetValue(statModifiers);
    }

    public int GetMaxAP() {
        return apmax.GetValue(statModifiers);
    }

    public int GetStrength() {
        return str.GetValue(statModifiers);
    }

    public int GetDefense() {
        return def.GetValue(statModifiers);
    }

    public int GetSpeed() {
        return spd.GetValue(statModifiers);
    }

    public int GetDexterity() {
        return dex.GetValue(statModifiers);
    }

    public int GetLuck() {
        return lck.GetValue(statModifiers);
    }

    public int GetMovement() {
        return mv.GetValue(statModifiers);
    }

    //resist a status effect when an attempt is made. The bool will return to false when the effect is resisted

    public void ResistCharm() {

        if(!charmRes){

            charmRes = true;
        }
    }

    public void ResistTaunt(){

        if(!tauntRes){

            tauntRes = true;
        }
    }
}
