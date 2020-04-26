using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is the GG Const file for Global Game Constant strings and other values
public class GGConst {
    public static string SCENE_NAME_SPLASH = "LoadingScreen";
    public static string SCENE_NAME_LOADING = "LoadingScreen";
    public static string SCENE_NAME_START = "Game_Title_Screen";
    public static string SCENE_NAME_DEBUG_START = "DebugStart";
    public static string SCENE_NAME_LEVEL_SELECT = "LevelSelect";
    public static string SCENE_NAME_WORKSHOP = "LanderWorkshop";
    public static string SCENE_NAME_LEVEL_PREFIX = "Tut_lvl_";

    public static string RESOURCE_PATH_PREFABS = "Prefabs/";
    public static string RESOURCE_PATH_ROOT = "Resources/";
    public static string PATH_DOT = ".";

    public static string INPUT_BUTTON_NAME_FIRE1 = "Fire1";

	public static string CHARACTER_OBJECT_NAME_PREFIX = "Character-";

	public static string TAG_MAIN_CAMERA = "MainCamera";
	public static string TAG_UI_GAMEPLAY_CANVAS = "UICanvasGameplay";
	public static string TAG_PLAYER = "Player";
	public static string TAG_RESCUE_ASTRONAUT = "FriendlyAstronaut";
	public static string TAG_CAPTURE_ORB = "CaptureOrb";
	public static string TAG_ENEMY_ROBOT_SMALL = "EnemyRobotSmall";
	public static string TAG_ENEMY_STRUCTURE = "EnemyStructure";
	public static string TAG_PLAYER_START = "PlayerStart";
	public static string TAG_LEVEL_BOUNDS = "Bounds";
	public static string TAG_CAMERA_BOUNDS = "CameraBounds";

    public static string SAVE_FILE_NAME_GAME_DATA = "gamedata";
    public static string SAVE_FILE_NAME_PLAYER_DATA = "playerdata_";
    public static string SAVE_FILE_NAME_DEBUG_DATA = "debugsavedata";
    public static string SAVE_FILE_EXT_JSON = ".json";
    public static string SAVE_FILE_EXT_BINARY = ".dat";

    public static string DATA_KEY_LAST_STATIC_VERSION = "StaticDataLastVersion";
    public static string DATA_KEY_LAST_PERSISTENT_PK = "PersistentDataLastPK";
    public static string DATA_KEY_CURRENT_PLAYER_PK = "CurrentPlayerPK";
    public static string PLAYER_DATA_KEY_CHARACTER_SELECT = "CharacterSelection";

    public static int DATA_PK_DEFAULT_PLAYER_PK = 1;
    public static string DATA_PK_DEFAULT_PLAYER = "Player01";
    public static string DATA_PK_DEFAULT_GAME_ROOT = "DefaultGameData01";
    public static int DATA_PK_DEFAULT_LANDER_PK = 1;
    public static string DATA_PK_DEFAULT_LANDER_NAME = "DefaultLander";
    public static string DATA_PK_DEFAULT_LANDER_DNAME = "Default Lander";
    public static string DATA_PK_DEFAULT_LANDER_PART_LG_PK = "LandingGear01";
    public static string DATA_PK_DEFAULT_LANDER_PART_HULL_PK = "Hull01";
    public static string DATA_PK_DEFAULT_LANDER_PART_WEAPON_PK = "Weapon01";
    public static string DATA_PK_DEFAULT_LANDER_PART_STRUT_PK = "Strut01";
    public static string DATA_PK_DEFAULT_LANDER_PART_THRUSTER_PK = "Thruster01";
    public static string DATA_PK_DEFAULT_LANDER_PART_ENGINE_PK = "Engine01";

    //new naming conventions:
    public static string ANIM_CHARACTER_SUFFIX_LEFT = "_left";
	public static string ANIM_CHARACTER_SUFFIX_RIGHT = "_right";
	public static string ANIM_CHARACTER_NONE_SUFFIX = "";
	public static string ANIM_CHARACTER_IDLE_PREFIX = "idle";
	public static string ANIM_CHARACTER_READY_PREFIX = "ready";
	public static string ANIM_CHARACTER_SHOT_PREFIX = "swing";
	public static string ANIM_EVENT_CHARACTER_BALL_HIT = "ballhit";

    //TIME CONTROL
    public static int TIME_SCALE_PAUSE = 0;
    public static int TIME_SCALE_RESUME = 1;
    public static float TIME_SCALE_SCREEN = 0.25f;
    public static float TIME_SLIDE_SCREEN = 0.5f;
}
