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
    public class TemplateGun : GunBehaviour
    {
        public static string internalName;
        public static int ID;
        public static void Add()
        {
            /* This basic template is a bare bones version of the Template Gun to provide a quick framework for making a gun without all of the clutter. */

            string FULLNAME = "Basic Gun";
            string SPRITENAME = "basicgun";
            internalName = $"{Plugin.MODPREFIX}:{FULLNAME.ToID()}";
            Gun gun = ETGMod.Databases.Items.NewGun(FULLNAME, SPRITENAME);
            Game.Items.Rename($"outdated_gun_mods:{FULLNAME.ToID()}", internalName);
            gun.gameObject.AddComponent<TemplateGunExpanded>();
            gun.SetShortDescription("I made a basic gun!");
            gun.SetLongDescription("A basic framework to build guns on.  Like a gun skeleton to give gun muscles and gun blood before draping in gun skin.");
            gun.SetupSprite(null, $"{SPRITENAME}_idle_001", 15);
            gun.SetAnimationFPS(gun.shootAnimation, 15);
            gun.SetAnimationFPS(gun.reloadAnimation, 15);
            gun.TrimGunSprites(); 
            tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName($"{SPRITENAME}_intro");
            gun.SetAnimationFPS(gun.introAnimation, 15);
            /* List of IDs and names: https://raw.githubusercontent.com/ModTheGungeon/ETGMod/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/items.txt
             * List of visual effects: https://enterthegungeon.wiki.gg/wiki/Weapon_Visual_Effects */
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(56) as Gun, true, false);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(56) as Gun).muzzleFlashEffects;
            /* List of default sound files: https://mtgmodders.gitbook.io/etg-modding-guide/various-lists-of-ids-sounds-etc./sound-list
             * Instructions on setting up custom sound files: https://mtgmodders.gitbook.io/etg-modding-guide/misc/using-custom-sounds */
            gun.gunSwitchGroup = $"{Plugin.MODPREFIX}_{FULLNAME.ToID()}";
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Shot_01", "Play_WPN_smileyrevolver_shot_01");
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Reload_01", "Play_WPN_crossbow_reload_01");
            gun.DefaultModule.angleVariance = 5;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.gunClass = GunClass.RIFLE;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.ammoCost = 1;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = 0.1f;
            gun.DefaultModule.numberOfShotsInClip = 15;
            gun.SetBaseMaxAmmo(150);
            gun.gunHandedness = GunHandedness.AutoDetect;
            gun.carryPixelOffset += new IntVector2(0, 0);
            gun.barrelOffset.transform.localPosition += new Vector3(0.75f, 0.2f);

            /* List of IDs and names: https://raw.githubusercontent.com/ModTheGungeon/ETGMod/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/items.txt
             * List of visual effects: https://enterthegungeon.wiki.gg/wiki/Weapon_Visual_Effects */
            Projectile projectile = gun.DefaultModule.projectiles[0].InstantiateAndFakeprefab();
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 5f;
            projectile.baseData.speed = 25f;
            projectile.baseData.range = 100f;
            projectile.baseData.force = 10f;
            projectile.transform.parent = gun.barrelOffset;

            projectile.SetProjectileSpriteRight($"{SPRITENAME}_projectile_001", 20, 8, true, tk2dBaseSprite.Anchor.MiddleCenter, 18, 6);

            /* List of ammo types: https://mtgmodders.gitbook.io/etg-modding-guide/various-lists-of-ids-sounds-etc./all-custom-ammo-types */
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "burning hand";

            gun.shellCasing = (PickupObjectDatabase.GetById(15)as Gun).shellCasing; //Example using AK-47 casings.
            gun.clipObject = (PickupObjectDatabase.GetById(15) as Gun).clipObject; //Example using AK-47 clips.
            gun.shellsToLaunchOnFire = 1;
            gun.clipsToLaunchOnReload = 1;

            /* Fire */
            projectile.AppliesFire = true;
            projectile.FireApplyChance = 0.5f;


            List<string> projectileSpriteNames = new List<string> { $"{SPRITENAME}_projectile_001", $"{SPRITENAME}_projectile_002", $"{SPRITENAME}_projectile_003", $"{SPRITENAME}_projectile_004", $"{SPRITENAME}_projectile_005" };
            int projectileFPS = 10;
            List<IntVector2> projectileSizes = new List<IntVector2> { new IntVector2(30, 12), new IntVector2(30, 12), new IntVector2(30, 12), new IntVector2(30, 12), new IntVector2(30, 12) };
            List<bool> projectileLighteneds = new List<bool> { true, true, true, true, true };
            List<tk2dBaseSprite.Anchor> projectileAnchors = new List<tk2dBaseSprite.Anchor>
                {tk2dBaseSprite.Anchor.MiddleCenter, tk2dBaseSprite.Anchor.MiddleCenter, tk2dBaseSprite.Anchor.MiddleCenter, tk2dBaseSprite.Anchor.MiddleCenter, tk2dBaseSprite.Anchor.MiddleCenter};
            List<bool> projectileAnchorsChangeColiders = new List<bool> { false, false, false, false, false };
            List<bool> projectilefixesScales = new List<bool> { false, false, false, false, false };
            List<Vector3?> projectileManualOffsets = new List<Vector3?> { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
            List<IntVector2?> projectileOverrideColliderSizes = new List<IntVector2?> { null, null, null, null, null };
            List<IntVector2?> projectileOverrideColliderOffsets = new List<IntVector2?> { null, null, null, null, null };
            List<Projectile> projectileOverrideProjectilesToCopyFrom = new List<Projectile> { null, null, null, null, null };
            tk2dSpriteAnimationClip.WrapMode ProjectileWrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;

            projectile.AddAnimationToProjectile(projectileSpriteNames, projectileFPS, projectileSizes, projectileLighteneds, projectileAnchors, projectileAnchorsChangeColiders, projectilefixesScales,
                                                projectileManualOffsets, projectileOverrideColliderSizes, projectileOverrideColliderOffsets, projectileOverrideProjectilesToCopyFrom, ProjectileWrapMode);

            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            gun.AddToSubShop(ItemBuilder.ShopType.Trorc);
            ID = gun.PickupObjectId;
        }
    }
}