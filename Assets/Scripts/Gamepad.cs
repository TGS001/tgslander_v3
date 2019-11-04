using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamepad {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    const int X_MOV = 1;
    const int Y_MOV = 2;
    const int X_POV = 3;
    const int Y_POV = 4;
    const int X_DPAD = -1;
    const int Y_DPAD = -1;
    const int B_TRIGGER = -1;
    const int L_TRIGGER = 5;
    const int R_TRIGGER = 6;

    const int BUTTON_A = 16;
    const int BUTTON_B = 17;
    const int BUTTON_X = 18;
    const int BUTTON_Y = 19;
    const int BUTTON_LBUMP = 13;
    const int BUTTON_RBUMP = 14;
    const int BUTTON_BACK = 10;
    const int BUTTON_START = 9;
    const int BUTTON_LSTICK = 11;
    const int BUTTON_RSTICK = 12;
    const int BUTTON_DPADUP = 5;
    const int BUTTON_DPADDOWN = 6;
    const int BUTTON_DPADLEFT = 7;
    const int BUTTON_DPADRIGHT = 8;
    const int BUTTON_XBOX = 15;
#endif

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
 //   const int X_MOV = 1;
	//const int Y_MOV = 2;
	//const int X_POV = 4;
	//const int Y_POV = 5;
	//const int X_DPAD = 6;
	//const int Y_DPAD = 7;
	//const int B_TRIGGER = 3;
	//const int L_TRIGGER = 9;
	//const int R_TRIGGER = 10;

	//const int BUTTON_A = 0;
	//const int BUTTON_B = 1;
	//const int BUTTON_X = 2;
	//const int BUTTON_Y = 3;
	//const int BUTTON_LBUMP = 4;
	//const int BUTTON_RBUMP = 5;
	//const int BUTTON_BACK = 6;
	//const int BUTTON_START = 7;
	//const int BUTTON_LSTICK = 8;
	//const int BUTTON_RSTICK = 9;
	//const int BUTTON_DPADUP = -1;
	//const int BUTTON_DPADDOWN = -1;
	//const int BUTTON_DPADLEFT = -1;
	//const int BUTTON_DPADRIGHT = -1;
	//const int BUTTON_XBOX = -1;
	#endif

	#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
	const int X_MOV = 1;
	const int Y_MOV = 2;
	const int X_POV = 4;
	const int Y_POV = 5;
	const int X_DPAD = 7;
	const int Y_DPAD = 8;
	const int B_TRIGGER = -1;
	const int L_TRIGGER = 3;
	const int R_TRIGGER = 6;

	const int BUTTON_A = 0;
	const int BUTTON_B = 1;
	const int BUTTON_X = 2;
	const int BUTTON_Y = 3;
	const int BUTTON_LBUMP = 4;
	const int BUTTON_RBUMP = 5;
	const int BUTTON_BACK = 6;
	const int BUTTON_START = 7;
	const int BUTTON_LSTICK = 9;
	const int BUTTON_RSTICK = 10;
	const int BUTTON_DPADUP = 13;
	const int BUTTON_DPADDOWN = 14;
	const int BUTTON_DPADLEFT = 11;
	const int BUTTON_DPADRIGHT = 12;
	const int BUTTON_XBOX = -1;
	#endif
}
