using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using Alexandria.SoundAPI;
using Alexandria.Misc;
using Alexandria.VisualAPI;
using Alexandria.BreakableAPI;
using BepInEx;
using System.Collections.Generic;


namespace Mod
{
    public class ExampleHandgun : AdvancedGunBehavior
    {
        public static string internalName;
        public static int ID;
        public static void Add()
        {
            /* This basic template is a bare bare bones version with comments of the Template Gun as a tutorial for new modders making guns for the first time. */

            /* NewGun(x,y) works where "x" is the full name of your gun and y is the prefix most of your sprite files use. 
             * Rename(a,b) works where "a" is what the game names your gun internally which uses lower case and underscores.  Here it would be "outdated_gun_mods:template_gun".
             * "b" is how you're renaming the gun to show up in the mod console.
             * The default here is to use your mod's prefix then shortname so in this example it would come out as "twp:template_gun". */
            string FULLNAME = "Example Handgun"; //Full name of your gun 
            string SPRITENAME = "example_handgun"; //The name that prefixes your sprite files
            internalName = $"{Plugin.MODPREFIX}:{FULLNAME.ToID()}"; 

            Gun gun = ETGMod.Databases.Items.NewGun(FULLNAME, SPRITENAME);
            Game.Items.Rename($"outdated_gun_mods:{FULLNAME.ToID()}", internalName); //Renames the default internal name to your custom internal name
            gun.gameObject.AddComponent<ExampleHandgun>(); //AddComponent<[ClassName]>

            gun.SetShortDescription("I made a basic gun!"); //The description that pops up when you pick up the gun.
            gun.SetLongDescription("A basic framework to learn how adding guns work.  It's a wooden training sword! But a gun. And still deadly."); //The full description in the Ammonomicon.

            /* SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.  Your "..._idle_001" is often a good example.  
             * A copy of the sprite used must be in your "sprites/Ammonomicon Encounter Icon Collection/" folder.
             * The variable at the end assigns a default FPS to all other animations. */
            gun.SetupSprite(null, $"{SPRITENAME}_idle_001", 15); 
            gun.SetAnimationFPS(gun.shootAnimation, 15);
            gun.SetAnimationFPS(gun.reloadAnimation, 15);
            gun.TrimGunSprites(); // ensures that any empty space in gun sprites is trimmed out and causes no issues :)

            /* List of IDs and names: https://raw.githubusercontent.com/ModTheGungeon/ETGMod/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/items.txt
             * List of visual effects: https://enterthegungeon.wiki.gg/wiki/Weapon_Visual_Effects */
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(56) as Gun).muzzleFlashEffects;

            /* gunSwitchGroup loads in the firing and reloading sound effects.
             * Use an existing ID if you want to copy another gun's firing and reloading sounds, otherwise use a custom gunSwitchGroup name then assign your sound effects manually.
             * List of default sound files https://mtgmodders.gitbook.io/etg-modding-guide/sounds/sound-list
             * Instructions on setting up custom sound files https://mtgmodders.gitbook.io/etg-modding-guide/sounds/using-custom-sounds 
             * You can have multiple sounds play at the same time! Useful for sounds like a gun spin. */
            gun.gunSwitchGroup = $"{Plugin.MODPREFIX}_{FULLNAME.ToID()}";
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Shot_01", "Play_WPN_smileyrevolver_shot_01"); // relace the last string here with the name of the sound you wish to play when the gun shoots.
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Reload_01", "Play_WPN_crossbow_reload_01", "Play_WPN_SAA_spin_01"); // same here but with reload sounds.

            gun.DefaultModule.angleVariance = 5; //How far from where you're aiming that bullets can deviate. 0 equals perfect accuracy.
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic; //Sets the firing style of the gun.
            gun.gunClass = GunClass.PISTOL; // Sets the gun's class which makes other guns of this class less common when this is in your inventory and vice versa. Sometimes affects category specific effects.
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random; //Sets how the gun handles multiple different projectiles
            gun.DefaultModule.ammoCost = 1;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = 0.1f; //Time between shots fired.  For Burst guns it's the time between each burst.
            gun.DefaultModule.numberOfShotsInClip = 15;
            gun.SetBaseMaxAmmo(150);

            gun.carryPixelOffset += new IntVector2(0, 0);
            /* BarrelOffset sets the length and width away on the sprite where the barrel should end.
             * This is where the muzzle flash and projectile will appear. A 1 is 16 pixels. */
            gun.barrelOffset.transform.localPosition += new Vector3(10/16f, 8/16f); // in this case the offset will be 14 pixels across and 8 pixels up.

            /* List of IDs and names: https://raw.githubusercontent.com/ModTheGungeon/ETGMod/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/items.txt
             * List of visual effects: https://enterthegungeon.wiki.gg/wiki/Weapon_Visual_Effects */
            Projectile projectile = gun.DefaultModule.projectiles[0].InstantiateAndFakeprefab(); /* InstantiateAndFakeprefab() ensures cloned copies of your projectile have the same properties. */
            gun.DefaultModule.projectiles[0] = projectile; //Assigns the projectile to the gun.

            /* Adjusting base properties
             * When tweaking the stats it's good to check against some basegame guns similar to your gun to get a feel for whats usual for that gun.
             * List of guns can be found here: https://enterthegungeon.wiki.gg/wiki/Guns 
             * But for immediate reference, Bullet Kin on floor 1 have a health of 15. */
            projectile.baseData.damage = 5f;
            projectile.baseData.speed = 25f;
            projectile.baseData.range = 100f;
            projectile.baseData.force = 10f; //Knockback strength
            projectile.transform.parent = gun.barrelOffset;

            /* Optionally sets a custom Projectile Sprite if you don't want to use the default.
            * The first value is the sprite name in sprites\ProjectileCollection without the extension.
            * tk2dBaseSprite.Anchor.MiddleCenter controls where the sprite is anchored. MiddleCenter will work in most cases.
            * The first set of numbers is visual dimensions of the sprite while the last set of numbers is the hitbox.  Generally the hitbox should be a little smaller than the visuals. */
            projectile.SetProjectileSpriteRight($"{SPRITENAME}_projectile_001", 7, 7, true, tk2dBaseSprite.Anchor.MiddleCenter, 5, 5);

            gun.shellCasing = BreakableAPIToolbox.GenerateDebrisObject("Mod/Resources/CustomDebris/example_handgun_shell").gameObject; //Example using a custom sprite as a casing (make sure the file path exists though).
            gun.clipObject = (PickupObjectDatabase.GetById(79) as Gun).clipObject; //Example using makarov clips.
            gun.shellsToLaunchOnFire = 1; //Number of shells to eject when shooting.
            gun.shellsToLaunchOnReload = 0; //Number of shells to eject when reloading (revolvers for example).
            gun.clipsToLaunchOnReload = 1; //Number of clips to eject when reloading.

            gun.quality = PickupObject.ItemQuality.D; //Sets the gun's quality rank. Use "SPECIAL" if the gun should not appear in chests but should appear in the ammonomicon (i.e. for starter weapons)
                                                      //Use "EXCLUDED" if your gun neither appears in chests or the ammonomicon..
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId; //Sets the Gun ID. 
        }
    }
}