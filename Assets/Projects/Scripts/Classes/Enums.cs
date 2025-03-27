using UnityEngine;

public enum ENEMY_STATE
{
    IDLE = 0,
    MOVING = 1,
    ATTACK = 2,
    GET_HIT = 3,
    DEAD = 4
}

public enum PLAYER_STATE
{
    ALIVE = 0,
    DEAD = 1
}

public enum GAME_STATE
{
    NONE = 0,
    MENU = 1,
    LOAD_MAP = 2,
    START_GAME = 3,
    GAME = 4,
    PAUSE = 5,
    COMPLETE = 6,
    GAME_OVER = 7
}

public enum UIEnum
{
    TITLE = 0,
    LOADING = 1,
    GAMEPLAY = 2,
}
