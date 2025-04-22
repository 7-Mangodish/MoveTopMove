using UnityEngine;

public class GameVariable : MonoBehaviour
{
    /*Do dai tieu chuan de co duoc scale tieu chuan
        Cach tinh: do dai duong chieu vuong goc cua CamToPlayerStatusVector len Vector(Camera.main.forward - GamePlayCam)
        Xem lai code CharacterStatus.cs;
     */
    public static float STD_DISTANCE = 2.861091f;
    public static Vector3 STD_SCALE = new Vector3(0.5f, 0.4f, 0.5f);

    public static string normalSceneName = "Map1";
    public static string zombieSceneName = "Map2";
    public static string zombieSplashSceneInName = "ZombieSplashScene_In";
    public static string zombieSplashSceneOutName = "ZombieSplashScene_Out";

    public static string ZOMBIE_DAY_VICTORY = "ZombieDayVictory";
    public static string PLAYER_COIN = "PlayerCoin";
    public static string PLAYER_SKIN = "PlayerSkin";
    public static string PLAYER_SKILL = "PlayerSkill";
    public static string PLAYER_PERSONAL_DATA = "PlayerPersonalData";


    public static string DEAD_TAG = "Untagged";
    public static string PLAYER_TAG = "Player";
    public static string ENEMY_TAG = "Enemy";
    public static string ZOMBIE_TAG = "Zombie";
    public static string CHARACTER_STATUS_TAG = "NameCharacter";

    public static string PLAYER_CURRENT_WEAPON = "CurWeapon";
    public static string AIM_AREA = "AimArea";

}
