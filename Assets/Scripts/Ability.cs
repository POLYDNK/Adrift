using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;

  [CreateAssetMenu] //allows creating of item assets directly unity by right clicking project
public class Ability : ScriptableObject
{
    private int totalDMG; //total damage dealt 
    private int totalHP; //total HP healed
    private int totalACC; //chance the ability hits
    public bool friendly; //Whether this ability targets allies or enemies (true for allies, false for enemies)
    public bool requiresTarget; //Whether this ability requires a selected target to execute
    public bool free; //Whether this ability consumes a turn action

    /*
    The below variables might be needed for the description generator :

    public bool willAttack; //Whether this ability will attack a target
    public bool buffsTarget; //Whether this ability buffs a target
    public bool buffsSelf; //Whether this ability buffs self
    public bool debuffsTarget; //Whether ability debuffs a target
    public bool debuffsSelf; //Whether ability debuffs self
    public bool healsTarget; //Whether ability heals a target
    public bool healsSelf; //whether this ability is a self heal
    public bool statusAilment; //whether this ability will inflict a status ailment
    public int totalDuration = 0; //determines total number of turns a buff/debuff will last for
    public string modifierList; //lists buff/debuff modifiers
    public string inflictedStatus; //status that will be inflicted

    */

    public int baseDMG; //ability will always do a certain amount of damage regardless of DEF
    public int baseHP; //ability will always heal a certain amount of HP
    public int baseACC; //ability comes with a set accuracy
    public int costAP; //AP cost
    public int range; //Distance from the user this ability can be used at
    public int knockback; //Movement to apply to targets (positive is push, negative is pull)
    public int selfMovement; //Movement to apply to user (positive is forwards, negative is backwards)
    public int totalHits = 1; //determines total number of hits for a given ability
    public string displayName; //Display name for UI
    public string description; //describes the ability in the UI
    public List<Vector2Int> shape; //Shape in tiles facing north (0,0) is the center
    public List<StatModifier> targetModifiers; //Stat modifiers to apply to targets
    public List<StatModifier> selfModifiers; //Stat modifiers to apply to user
    [SerializeField] public GameObject targetEffect; //Visual effect to apply to affected targets
    [SerializeField] public GameObject selfEffect; //Visual effect to apply to user
    

    // Start is called before the first frame update
    void Start()
    {

        //descriptionGenerator(descirption);

        



    }

    // Update is called once per frame
    void Update()
    {

    }

/*
    public void descriptionGenerator(string description){

        description = "[";
        if(baseDMG != 0){

            description += "BASE DMG: " + baseDMG + ". ";

        }
        if(baseHP != 0){

            description += "BASE HEAL: " + baseHP + ". ";
        }
        
        description += "RANGE: " + range + ". " + "COST: " + costAP + "] ";

        if(willAttack){

            description += "Attack the enemy";

            if(totalHits > 1){

                description += " " + totalHits + "times.";
            }
            else{

                descirption += ".";
            }
        }
        else if(healsSelf){

            description += "Heal the user.";

        }
        else if(healsTarget){

            description += "Heal an ally.";
        }
        else if(buffsSelf){

            decription += "Grants " + modifierList + " to the user for " + totalDuration + " turns.";
        }
        else if(buffsTarget){

             decription += "Grants " + modifierList + " to an ally for " + totalDuration + " turns.";
        }
        else if(debuffsSelf){

             decription += "Inflicts " + modifierList + " on the user for " + totalDuration + " turns.";
        }
        else if(debuffsTarget){

            decription += "Inflicts " + modifierList + " on the enemy for " + totalDuration + " turns.";
        }
        else if(statusAilment){

            description += "Inflicts [" + inflictedStatus + "] on the enemy for " + totalDuration + " turns.";
        }

    }
*/

    // Return knockback value rotated dependent on which way user is facing
    public Vector2Int getKnockback(int xDist, int yDist) {
        return rotateMovement(knockback, xDist, yDist);
    }

    // Return self movement value rotated dependent on which way user is facing
    public Vector2Int getSelfMovement(int xDist, int yDist) {
        return rotateMovement(selfMovement, xDist, yDist);
    }

    private Vector2Int rotateMovement(int magnitude, int xDist, int yDist) {
        if(Mathf.Abs(xDist) >= Mathf.Abs(yDist)) {
            if(xDist > 0) return new Vector2Int(-magnitude, 0);
            else return new Vector2Int(magnitude, 0);
        }
        else if(yDist > 0) return new Vector2Int(0, -magnitude);
        else return new Vector2Int(0, magnitude);
    }

    public Vector2Int applySelfMovement(CharacterStats character, GridBehavior grid, int xDist, int yDist) {
        if(selfMovement == 0) return character.gridPosition;
        return applyMovement(character, grid, rotateMovement(selfMovement, xDist, yDist));
    }

    public Vector2Int applyKnockback(CharacterStats character, GridBehavior grid, int xDist, int yDist) {
        if(knockback == 0) return character.gridPosition;
        return applyMovement(character, grid, rotateMovement(knockback, xDist, yDist));
    }

    private Vector2Int applyMovement(CharacterStats character, GridBehavior grid, Vector2Int movement) {
        //Look for longest path and try to take it
        while(movement.x != 0 || movement.y != 0) {
            Vector2Int pos = new Vector2Int(character.gridPosition.x + movement.x, character.gridPosition.y + movement.y);
            if(grid.GetLinearPath(character.gridPosition, pos)) return pos;
            if(movement.x < 0) movement.x++;
            else if(movement.x > 0) movement.x--;
            else if(movement.y < 0) movement.y++;
            else if(movement.y > 0) movement.y--;
        }
        return character.gridPosition;
    }

    // Return shape rotated dependent on which way user is facing
    public List<Vector2Int> getRelativeShape(int xDist, int yDist) {
        List<Vector2Int> list = new List<Vector2Int>();
        foreach(Vector2Int pos in shape) {
            //Rotate position relative to user
            if(Mathf.Abs(xDist) >= Mathf.Abs(yDist))
            {
                if(xDist > 0) list.Add(new Vector2Int(-pos.y, pos.x)); //West
                else list.Add(new Vector2Int(pos.y, pos.x)); //East
            }
            else if(yDist > 0) list.Add(new Vector2Int(-pos.x, -pos.y)); //South
            else list.Add(pos); //North
        }
        return list;
    }

    // Apply ability to target character
    public void affectCharacter(GameObject user, GameObject target, int currentHit) {
        callAbility(user.GetComponent<CharacterStats>(), target.GetComponent<CharacterStats>(), currentHit);
    }

    // Apply ability to target list of characters
    public void affectCharacters(GameObject user, List<GameObject> targets, int currentHit) {
        foreach(GameObject target in targets) {
            callAbility(user.GetComponent<CharacterStats>(), target.GetComponent<CharacterStats>(), currentHit);
        }
    }

    //call an ability based on given display name
    public void callAbility(CharacterStats user, CharacterStats target, int currentHit){
        if(selfEffect != null) {
            GameObject particle = Instantiate(selfEffect, user.transform);
            particle.transform.position = user.transform.position;
        }

        /// COMBO ///

        if(displayName == "Combo")                  Combo(user, target);


        /// WEAPON ///


        else if(displayName == "Pierce")            Pierce(user, target);
        else if(displayName == "Shooting Star")     ShootingStar(user, target);
        else if(displayName == "Siphon")            Siphon(user, target);
        else if(displayName == "Life Swap")         LifeSwap(user, target);
        else if(displayName == "Soul Bash")         SoulBash(user, target);
        else if(displayName == "Persistence")       Persistence(user, target);
        else if(displayName == "Light Heal")        LightHeal(user);
        else if(displayName == "Quick Attack")      QuickAttack(user, target);
        else if(displayName == "Gambit")            Gambit(user, target, currentHit);


                   

        /// PIRATE ////

        else if(displayName == "Pistol Shot")       PistolShot(user, target);
        else if(displayName == "Head Shot")         HeadShot(user, target);
        else if(displayName == "Flank")             Flank(user);

        /// BARD ///

        else if(displayName == "Inspiring Shanty")  InspiringShanty(user, target);
        else if(displayName == "Reckless Tune")     RecklessTune(user, target);
        else if(displayName == "Maddening Jig")     MaddeningJig(user, target);

        /// MERCENARY ///


        else if(displayName == "Provoke")           Provoke(user, target);
        else if(displayName == "Reeling Chains")    ReelingChains(user, target);
        else if(displayName == "Incarcerate")       Incarcerate(user, target);


        /// MERCHANT ///


        else if(displayName == "Disparage")         Disparage(user, target);
        else if(displayName == "Promise of Coin")   PromiseOfCoin(user, target);
        else if(displayName == "On The House")      OnTheHouse(user, target);


        /// BUCCANEER ///


        else if(displayName == "Rifle Shot")        RifleShot(user, target);
        else if(displayName == "Cheap Shot")        CheapShot(user, target);
        else if(displayName == "Explosive Blast")   ExplosiveBlast(user, target);


        /// ACOLYTE ///


        else if(displayName == "Replenish")         Replenish(user, target);
        else if(displayName == "Noble Rite")        NobleRite(user, target);
        else if(displayName == "Purge")             Purge(user, target);


        /// PIRATE CAPTAIN ///


        else if(displayName == "Pistol Volley")     PistolVolley(user, target);
        else if(displayName == "Death Wish")        DeathWish(user, target);
        else if(displayName == "Second Wind")       SecondWind(user);
        else if(displayName == "Whirling Steel")    WhirlingSteel(user, target);


        /// MERCHANT CAPTAIN ///


        else if(displayName == "Tempt")             Tempt(user, target);
        else if(displayName == "Forceful Lashing")  ForcefulLashing(user, target);
        else if(displayName == "Harrowing Speech")  HarrowingSpeech(user, target);
        else if(displayName == "Silver Tongue")     SilverTongue(user, target);


        /// MONARCHY CAPTAIN ///


        else if(displayName == "Command")           Command(user, target);
        else if(displayName == "Shield Bash")       ShieldBash(user, target);
        else if(displayName == "Enrage")            Enrage(user, target);
        else if(displayName == "King's Will")       KingsWill(user);


        /// BASIC ///
        else Generic(user, target);

    }

    //Ability implementations

//// GENERAL ABILITIES ///

    //Generic ability with no special effects
    void Generic(CharacterStats user, CharacterStats target) {
        if(!friendly)
        {//attack the enemy
            if(user.weapon != null && (user.weapon.speedSlash && (user.getSpeed() - target.getSpeed() >= 5))){

                int bonusDMG = ((user.getSpeed() - target.getSpeed()) / 5) * 2;

                GameObject hitParticle = Instantiate(targetEffect);
                hitParticle.transform.position = target.transform.position;

                totalDMG = user.Attack(target, 1, bonusDMG, 0, 0) + baseDMG;

                target.adjustHP(-totalDMG, false);

                
            }
            else
            {
                GameObject hitParticle = Instantiate(targetEffect);
                hitParticle.transform.position = target.transform.position;

                totalDMG = user.Attack(target, 1, 0, 0, 0) + baseDMG;

                target.adjustHP(-totalDMG, false);
            }
        }
        else
        {//heal an ally

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalHP = user.Heal(target);

            target.adjustHP(totalHP + baseHP, true);
        }
        //Apply modifiers to self
        foreach(StatModifier modifier in selfModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier);
        }
        //Apply modifiers to target
        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier);
        }
    }

    //combo attacks will ignore defense if they hit
    void Combo(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalDMG = (baseDMG + user.getATK(target, true));

            target.adjustHP(-totalDMG, false);


        }

    }


/// PIRATE CLASS ABILITIES ///

    //attack the enemy, does not crit
    void PistolShot(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalDMG = user.getATK(target, false) + baseDMG;

            target.adjustHP(-totalDMG, false);


        }
        


    }

    //attack the enemy with high damage, ignores defense, does not crit
    void HeadShot(CharacterStats user, CharacterStats target){


        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalDMG = user.getATK(target, true) + baseDMG;

            target.adjustHP(-totalDMG, false);


        }



        
    }



    //Move to any tile around adjacent enemeies, also grants +2 SPD and DEX for 2 turns
    void Flank(CharacterStats user){

        //effect

        foreach(StatModifier modifier in selfModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier.clone());
        }

    }


/// BARD CLASS ABILITIES ///

    //buffs the target's STR and SPD for 3 turns (+5). Friendly ability
    void InspiringShanty(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
        }



    }

    //heal the target and grant +1 AP, but debuff defense for 3 turns (-3). Friendly ability
    void RecklessTune(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalHP = baseHP + user.Heal(target);

        target.adjustHP(totalHP, true);

        target.adjustAP(1,0);

        
        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
        }



    }


    //if ability hits, debuff target's DEF (-5) and accuraccy (DEX) (-5) for 3 turns
    void MaddeningJig(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        if(user.determineHIT(baseACC)){

            //possible effect

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
            }
        }

    }


/// MERCENARY CLASS ABILITIES ///

    //Taunt: +5 STR/SPD / -10 DEF/LCK/DEX to target. Lasts for 1 turn
    void Provoke(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            if(!target.tauntRES){

                foreach(StatModifier modifier in targetModifiers) {
                    if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
                }


            }
            else{

                target.tauntRES = false;
            }
        }

    }

    //Low damage, pull enemy towards the user, also grants +3 DEX to user for 3 turns
    void ReelingChains(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalDMG = baseDMG + (user.getDexterity() / 3);

            target.adjustHP(-totalDMG, false);

            foreach(StatModifier modifier in selfModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier.clone());
            }
        }

    }



    //medium damage, debuff SPD (-8) and MV (-3) to target for 2 turns
    void Incarcerate(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalDMG = baseDMG + (user.getDexterity() / 2);

            target.adjustHP(-totalDMG, false);

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
            }
        }
 
    }



/// MERCHANT CLASS ABILITIES ///

    // -6 STR -4 DEX to target for 1 turns
    void Disparage(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            //effect

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
            }
        }
        
    }



    // +6 DEF +4 LCK to target for 2 turns
    void PromiseOfCoin(CharacterStats user, CharacterStats target){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            //effect

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
            }
    
    }

    
    //Grants one random buff to target for 3 turns (if move is successful)
    //Can choose from any stat (minus MV, AP, or HP)
    // +3 STR (90% chance)
    // +3 DEF (50% chance)
    // +3 SPD (30% chance)
    // +3 DEX (10% chance)
    // +3 LCK (1% chance)
    void OnTheHouse(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        //effect

        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
        }
    
    }





/// BUCCANEER CLASS ABILITIES ///

    //moderate damage, can convert remaining movement to +5 ACC
    void RifleShot(CharacterStats user, CharacterStats target){


        totalACC = baseACC; //+ remaining movement * 5
        if(user.determineHIT(totalACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            target.adjustHP(-baseDMG, false);

        }

        
    }


    //low damage, -2 MV to target
    void CheapShot(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            target.adjustHP(-baseDMG, false);

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
            }
        }
        
    }


    //medium damage if move successful
    void ExplosiveBlast(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            target.adjustHP(-baseDMG, false);

        }
        
    }




/// ACOLYTE CLASS ABILITIES ///

    //heal the target (moderate)
    void Replenish(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalHP = user.Heal(target) + baseHP;

        target.adjustHP(totalHP, true);

        
    }



    //heal the target (strong) and grant +6 LCK
    void NobleRite(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalHP = user.Heal(target) + baseHP + target.getLuck();

        target.adjustHP(totalHP, true);

        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
        }

        
    }


    //clear all debuffs and grant 1 AP to target
    void Purge(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        if(user.determineHIT(baseACC)){

            target.clearDebuffs();

            target.adjustAP(1,0);

        }

    }


/// GENERAL WEAPON ABILITIES ///

    //Attack the enemy and then heal (= Attack Power)
    void Siphon(CharacterStats user, CharacterStats target){

        int modifier = 0;

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalDMG = user.Attack(target, 1, 0, 0, 0);

        target.adjustHP(-totalDMG, false);

        if(user.weapon != null && user.weapon.strongSiphon){//Strong Siphon gives 20% more HP

            modifier = (totalDMG * 2) / 10;
        }

        user.adjustHP(totalDMG + modifier, true);

    }

    //attack while ignorning enemy defenses (base damage = 10)
    void Pierce(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        if(user.weapon != null && user.weapon.deadlyPierce){

            totalDMG = user.Attack(target, 2, 0, 0, 15) + baseDMG;
            target.adjustHP(-totalDMG, false);


        }
        else{

            totalDMG = user.Attack(target, 2, 0, 0, 0) + baseDMG;
            target.adjustHP(-totalDMG, false);


        }


        

    }

    //attack the enemy 5 times, with each attack having 1/5 the power of a normal attack
    void ShootingStar(CharacterStats user, CharacterStats target){

        /* if(user.weapon != null && user.weapon.shiningStar){ */

            GameObject hitParticle = Instantiate(targetEffect, target.transform);

            totalDMG = (user.Attack(target, 2, 0, 0, 15) / 5) + baseDMG;
            target.adjustHP(-totalDMG, false);

        /* 
        }
        
        else{

            GameObject hitParticle = Instantiate(targetEffect, target.transform);
            hitParticle.transform.position = target.transform.position;

            totalDMG = user.Attack(target, 1, 0, 0, 0) / 5;
            target.adjustHP(-totalDMG, false);

            
        }
        */

    }

    //Swap current HP with the target.
    //This move does not kill the user/target (reduces HP to a max of 1)
    void LifeSwap(CharacterStats user, CharacterStats target){

        int DEX = user.getDexterity(), STR = user.getStrength(), LCK = user.getLuck();
        totalACC = ((((DEX * 3 + LCK) / 2) + (2 * (user.Morale / 20))) - target.AVO) + user.accessoryBonus(1) + baseACC;

        int modifier = 0;

        if(user.determineHIT(totalACC)){

            totalHP = target.HP - user.HP;

            if(user.weapon != null && user.weapon.safeSwap){

                modifier = user.Heal(user);

            }

            user.adjustHP(totalHP + modifier, true);
            target.adjustHP(totalHP, true);
        }
    }

    //Attack the target. Bonus dmg is applied depending on current HP (more HP, more dmg)
    void SoulBash(CharacterStats user, CharacterStats target){

        baseDMG = user.Attack(target, 1, 0, 0, 0);

        if(user.weapon != null && user.weapon.strongSoul && user.isFullHealth()){

            totalDMG = baseDMG + user.HP;

        }
        else{

            totalDMG = baseDMG + (user.HP / 3);

        }

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        target.adjustHP(-totalDMG, false);


    }

    //Attack the target. Bonus dmg is applied depending on current HP (less HP, more dmg)
    void Persistence(CharacterStats user, CharacterStats target){

        baseDMG = user.Attack(target, 1, 0, 0, 0);

        totalDMG = baseDMG + (user.getMaxHP() - user.HP);

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        target.adjustHP(-totalDMG, false);
    }


    //heal self. Heal more if passive is active or class = Acolyte
    void LightHeal(CharacterStats user){

        if(user.weapon != null && (user.weapon.betterHeal || user.classname == className.Acolyte)){

            baseHP *= 2;
        }

        totalHP = user.Heal(user) + baseHP;

        user.adjustHP(totalHP, true);
    }

    //attack the target with 50% user's ATK with bonus dmg = user's SPD.
    void QuickAttack(CharacterStats user, CharacterStats target){

        if(user.weapon != null && user.weapon.certainty){//attack doesn't miss

            totalDMG = (user.Attack(target, 3, 0, 0, 0) / 2) + user.getSpeed();


        }
        else{

            totalDMG = (user.Attack(target, 1, 0, 0, 0) / 2) + user.getSpeed();


        }

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        target.adjustHP(-totalDMG, false);
    }

    //Attack the enemy 1 + 10 times. The first attack never misses
    //This ability can't crit
    //This ability starts at 20 damage, and gets +5 damage each attempt, for a max of 70 damage
    //This ability will end when the target dies or when all hits were attempted
    void Gambit(CharacterStats user, CharacterStats target, int currentHit){

        int ACC = 0;
        int DMG = 0;

        if(currentHit == 0){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            target.adjustHP(-20, false);

            return;
        }
        else{

            DMG = 20 + (5 * currentHit);

            if(user.weapon != null && user.weapon.earlyGambit){

                ACC = 95 - (5 * currentHit);

            }
            else if(user.weapon != null && user.weapon.lateGambit){

                ACC = 55 + (5 * currentHit);

            }
            else{

                ACC = 70;

            }

            if(user.determineHIT(ACC)){

                GameObject hitParticle = Instantiate(targetEffect);
                hitParticle.transform.position = target.transform.position;

                target.adjustHP(-DMG, false);

            }
        }


    }
        
        
    





    


/// PIRATE CAPTAIN ABILITIES ///

    void WhirlingSteel(CharacterStats user, CharacterStats target){

        //blank for now
        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;


        target.adjustHP(-totalDMG - baseDMG, false);
    }

    void SecondWind(CharacterStats user){

        GameObject hitParticle = Instantiate(targetEffect, user.transform);


        totalHP = user.Heal(user) + baseHP;
        user.adjustHP(totalHP, true);
    }

    void PistolVolley(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalDMG = baseDMG;
        target.adjustHP(-totalDMG, false);
        
    }

    void DeathWish(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            target.adjustHP(-totalDMG - baseDMG, false);

            if(target.isDead()){

                user.addAP(1);
            }




        }





    }




/// MERCHANT CAPTAIN ABILITIES ///



    //-8 DEF to target for 3 turns, user resists next enemy Charm
    void Tempt(CharacterStats user, CharacterStats target){


        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
        }

        user.resistCharm();
        

    }



    //Low damage, knockback 1, inflicts [Taunt]
    //Taunt: +5 STR/SPD / -10 DEF/LCK/DEX to target for 1 turn
    void ForcefulLashing(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            target.adjustHP(-baseDMG, false);

            if(!target.tauntRES){

                foreach(StatModifier modifier in targetModifiers) {
                    if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
                }


            }
            else{

                target.tauntRES = false;
            }

        }

        
    }


    //Lower enemy morale by 3 for each target hit
    void HarrowingSpeech(CharacterStats user, CharacterStats target){

        target.adjustMorale(-3);
        
    }



    //Inflict [Charm] depending on enemy morale
    //Charm: -6 STR/DEF/DEX for 1 turn
    void SilverTongue(CharacterStats user, CharacterStats target){

        if(user.determineHIT(target.Morale)){

            if(!target.charmRES){

                foreach(StatModifier modifier in targetModifiers) {
                    if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
                }


            }
            else{

                target.charmRES = false;
            }

        }
 
    }








/// MONARCHY CAPTAIN ABILITIES ///



    //+7 STR/DEX/LCK to target for 2 turns
    void Command(CharacterStats user, CharacterStats target){

        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
        }

   
    }


    //medium damage, knockback 3, self forward 1
    //if armor is equipped, bonus damage = user's DEF / 3
    void ShieldBash(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            int bonusDMG = 0;

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            if(user.armor != null){

                bonusDMG = user.getDefense() / 3;
            }

            totalDMG = baseDMG + bonusDMG;

            target.adjustHP(-totalDMG, false);

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
            }
        }
        
    }


    //inflicts [Taunt]
    //Taunt: +5 STR/SPD / -10 DEF/LCK/DEX to target for 1 turn
    void Enrage(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            if(!target.tauntRES){

                foreach(StatModifier modifier in targetModifiers) {
                    if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
                }


            }
            else{

                target.tauntRES = false;
            }
        }
        
    }



    //+3 STR/DEX and immune to damage for 1 turn
    void KingsWill(CharacterStats user){

        user.DMGimmune = true;

        GameObject hitParticle = Instantiate(selfEffect);
        hitParticle.transform.position = user.transform.position;

        foreach(StatModifier modifier in selfModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier.clone());
        }
   
    }





    
}