﻿using System;

namespace ConsoleGameEngine {
	/// <summary> Enum for basic Unicodes. </summary>
	public enum ConsoleCharacter {
		Null = 0x0000,

		Full = 0x2588,
		Dark = 0x2593,
		Medium = 0x2592,
		Light = 0x2591,

		// box drawing syboler
		// ┌───────┐
		// │       │
		// │       │
		// └───────┘
		BoxDrawingL_H = 0x2500,
		BoxDrawingL_V = 0x2502,
		BoxDrawingL_DR = 0x250C,
		BoxDrawingL_DL = 0x2510,
		BoxDrawingL_UL = 0x2518,
		BoxDrawingL_UR = 0x2514,
	}

	/// <summary> Enum for Different Gameloop modes. </summary>
	public enum FramerateMode {
		/// <summary>Run at max speed, but no higher than TargetFramerate.</summary>
		MaxFps,
		/// <summary>Run at max speed.</summary>
		Unlimited
	}

	/// <summary> Represents prebuilt palettes. </summary>
	public static class Palettes 
    {
		/// <summary> default windows console palette. </summary>
		public static Color[] Default { get; set; } = new Color[256] 
        {
            new Color(0x00, 0x00, 0x00),
            new Color(0x00, 0x00, 0xaa),
            new Color(0x00, 0xaa, 0x00),
            new Color(0x00, 0xaa, 0xaa),
            new Color(0xaa, 0x00, 0x00),
            new Color(0xaa, 0x00, 0xaa),
            new Color(0xaa, 0x55, 0x00),
            new Color(0xaa, 0xaa, 0xaa),
            new Color(0x55, 0x55, 0x55),
            new Color(0x55, 0x55, 0xff),
            new Color(0x55, 0xff, 0x55),
            new Color(0x55, 0xff, 0xff),
            new Color(0xff, 0x55, 0x55),
            new Color(0xff, 0x55, 0xff),
            new Color(0xff, 0xff, 0x55),
            new Color(0xff, 0xff, 0xff),
            new Color(0x00, 0x00, 0x00),
            new Color(0x14, 0x14, 0x14),
            new Color(0x20, 0x20, 0x20),
            new Color(0x2c, 0x2c, 0x2c),
            new Color(0x38, 0x38, 0x38),
            new Color(0x45, 0x45, 0x45),
            new Color(0x51, 0x51, 0x51),
            new Color(0x61, 0x61, 0x61),
            new Color(0x71, 0x71, 0x71),
            new Color(0x82, 0x82, 0x82),
            new Color(0x92, 0x92, 0x92),
            new Color(0xa2, 0xa2, 0xa2),
            new Color(0xb6, 0xb6, 0xb6),
            new Color(0xcb, 0xcb, 0xcb),
            new Color(0xe3, 0xe3, 0xe3),
            new Color(0xff, 0xff, 0xff),
            new Color(0x00, 0x00, 0xff),
            new Color(0x41, 0x00, 0xff),
            new Color(0x7d, 0x00, 0xff),
            new Color(0xbe, 0x00, 0xff),
            new Color(0xff, 0x00, 0xff),
            new Color(0xff, 0x00, 0xbe),
            new Color(0xff, 0x00, 0x7d),
            new Color(0xff, 0x00, 0x41),
            new Color(0xff, 0x00, 0x00),
            new Color(0xff, 0x41, 0x00),
            new Color(0xff, 0x7d, 0x00),
            new Color(0xff, 0xbe, 0x00),
            new Color(0xff, 0xff, 0x00),
            new Color(0xbe, 0xff, 0x00),
            new Color(0x7d, 0xff, 0x00),
            new Color(0x41, 0xff, 0x00),
            new Color(0x00, 0xff, 0x00),
            new Color(0x00, 0xff, 0x41),
            new Color(0x00, 0xff, 0x7d),
            new Color(0x00, 0xff, 0xbe),
            new Color(0x00, 0xff, 0xff),
            new Color(0x00, 0xbe, 0xff),
            new Color(0x00, 0x7d, 0xff),
            new Color(0x00, 0x41, 0xff),
            new Color(0x7d, 0x7d, 0xff),
            new Color(0x9e, 0x7d, 0xff),
            new Color(0xbe, 0x7d, 0xff),
            new Color(0xdf, 0x7d, 0xff),
            new Color(0xff, 0x7d, 0xff),
            new Color(0xff, 0x7d, 0xdf),
            new Color(0xff, 0x7d, 0xbe),
            new Color(0xff, 0x7d, 0x9e),
            new Color(0xff, 0x7d, 0x7d),
            new Color(0xff, 0x9e, 0x7d),
            new Color(0xff, 0xbe, 0x7d),
            new Color(0xff, 0xdf, 0x7d),
            new Color(0xff, 0xff, 0x7d),
            new Color(0xdf, 0xff, 0x7d),
            new Color(0xbe, 0xff, 0x7d),
            new Color(0x9e, 0xff, 0x7d),
            new Color(0x7d, 0xff, 0x7d),
            new Color(0x7d, 0xff, 0x9e),
            new Color(0x7d, 0xff, 0xbe),
            new Color(0x7d, 0xff, 0xdf),
            new Color(0x7d, 0xff, 0xff),
            new Color(0x7d, 0xdf, 0xff),
            new Color(0x7d, 0xbe, 0xff),
            new Color(0x7d, 0x9e, 0xff),
            new Color(0xb6, 0xb6, 0xff),
            new Color(0xc7, 0xb6, 0xff),
            new Color(0xdb, 0xb6, 0xff),
            new Color(0xeb, 0xb6, 0xff),
            new Color(0xff, 0xb6, 0xff),
            new Color(0xff, 0xb6, 0xeb),
            new Color(0xff, 0xb6, 0xdb),
            new Color(0xff, 0xb6, 0xc7),
            new Color(0xff, 0xb6, 0xb6),
            new Color(0xff, 0xc7, 0xb6),
            new Color(0xff, 0xdb, 0xb6),
            new Color(0xff, 0xeb, 0xb6),
            new Color(0xff, 0xff, 0xb6),
            new Color(0xeb, 0xff, 0xb6),
            new Color(0xdb, 0xff, 0xb6),
            new Color(0xc7, 0xff, 0xb6),
            new Color(0xb6, 0xff, 0xb6),
            new Color(0xb6, 0xff, 0xc7),
            new Color(0xb6, 0xff, 0xdb),
            new Color(0xb6, 0xff, 0xeb),
            new Color(0xb6, 0xff, 0xff),
            new Color(0xb6, 0xeb, 0xff),
            new Color(0xb6, 0xdb, 0xff),
            new Color(0xb6, 0xc7, 0xff),
            new Color(0x00, 0x00, 0x71),
            new Color(0x1c, 0x00, 0x71),
            new Color(0x38, 0x00, 0x71),
            new Color(0x55, 0x00, 0x71),
            new Color(0x71, 0x00, 0x71),
            new Color(0x71, 0x00, 0x55),
            new Color(0x71, 0x00, 0x38),
            new Color(0x71, 0x00, 0x1c),
            new Color(0x71, 0x00, 0x00),
            new Color(0x71, 0x1c, 0x00),
            new Color(0x71, 0x38, 0x00),
            new Color(0x71, 0x55, 0x00),
            new Color(0x71, 0x71, 0x00),
            new Color(0x55, 0x71, 0x00),
            new Color(0x38, 0x71, 0x00),
            new Color(0x1c, 0x71, 0x00),
            new Color(0x00, 0x71, 0x00),
            new Color(0x00, 0x71, 0x1c),
            new Color(0x00, 0x71, 0x38),
            new Color(0x00, 0x71, 0x55),
            new Color(0x00, 0x71, 0x71),
            new Color(0x00, 0x55, 0x71),
            new Color(0x00, 0x38, 0x71),
            new Color(0x00, 0x1c, 0x71),
            new Color(0x38, 0x38, 0x71),
            new Color(0x45, 0x38, 0x71),
            new Color(0x55, 0x38, 0x71),
            new Color(0x61, 0x38, 0x71),
            new Color(0x71, 0x38, 0x71),
            new Color(0x71, 0x38, 0x61),
            new Color(0x71, 0x38, 0x55),
            new Color(0x71, 0x38, 0x45),
            new Color(0x71, 0x38, 0x38),
            new Color(0x71, 0x45, 0x38),
            new Color(0x71, 0x55, 0x38),
            new Color(0x71, 0x61, 0x38),
            new Color(0x71, 0x71, 0x38),
            new Color(0x61, 0x71, 0x38),
            new Color(0x55, 0x71, 0x38),
            new Color(0x45, 0x71, 0x38),
            new Color(0x38, 0x71, 0x38),
            new Color(0x38, 0x71, 0x45),
            new Color(0x38, 0x71, 0x55),
            new Color(0x38, 0x71, 0x61),
            new Color(0x38, 0x71, 0x71),
            new Color(0x38, 0x61, 0x71),
            new Color(0x38, 0x55, 0x71),
            new Color(0x38, 0x45, 0x71),
            new Color(0x51, 0x51, 0x71),
            new Color(0x59, 0x51, 0x71),
            new Color(0x61, 0x51, 0x71),
            new Color(0x69, 0x51, 0x71),
            new Color(0x71, 0x51, 0x71),
            new Color(0x71, 0x51, 0x69),
            new Color(0x71, 0x51, 0x61),
            new Color(0x71, 0x51, 0x59),
            new Color(0x71, 0x51, 0x51),
            new Color(0x71, 0x59, 0x51),
            new Color(0x71, 0x61, 0x51),
            new Color(0x71, 0x69, 0x51),
            new Color(0x71, 0x71, 0x51),
            new Color(0x69, 0x71, 0x51),
            new Color(0x61, 0x71, 0x51),
            new Color(0x59, 0x71, 0x51),
            new Color(0x51, 0x71, 0x51),
            new Color(0x51, 0x71, 0x59),
            new Color(0x51, 0x71, 0x61),
            new Color(0x51, 0x71, 0x69),
            new Color(0x51, 0x71, 0x71),
            new Color(0x51, 0x69, 0x71),
            new Color(0x51, 0x61, 0x71),
            new Color(0x51, 0x59, 0x71),
            new Color(0x00, 0x00, 0x41),
            new Color(0x10, 0x00, 0x41),
            new Color(0x20, 0x00, 0x41),
            new Color(0x30, 0x00, 0x41),
            new Color(0x41, 0x00, 0x41),
            new Color(0x41, 0x00, 0x30),
            new Color(0x41, 0x00, 0x20),
            new Color(0x41, 0x00, 0x10),
            new Color(0x41, 0x00, 0x00),
            new Color(0x41, 0x10, 0x00),
            new Color(0x41, 0x20, 0x00),
            new Color(0x41, 0x30, 0x00),
            new Color(0x41, 0x41, 0x00),
            new Color(0x30, 0x41, 0x00),
            new Color(0x20, 0x41, 0x00),
            new Color(0x10, 0x41, 0x00),
            new Color(0x00, 0x41, 0x00),
            new Color(0x00, 0x41, 0x10),
            new Color(0x00, 0x41, 0x20),
            new Color(0x00, 0x41, 0x30),
            new Color(0x00, 0x41, 0x41),
            new Color(0x00, 0x30, 0x41),
            new Color(0x00, 0x20, 0x41),
            new Color(0x00, 0x10, 0x41),
            new Color(0x20, 0x20, 0x41),
            new Color(0x28, 0x20, 0x41),
            new Color(0x30, 0x20, 0x41),
            new Color(0x38, 0x20, 0x41),
            new Color(0x41, 0x20, 0x41),
            new Color(0x41, 0x20, 0x38),
            new Color(0x41, 0x20, 0x30),
            new Color(0x41, 0x20, 0x28),
            new Color(0x41, 0x20, 0x20),
            new Color(0x41, 0x28, 0x20),
            new Color(0x41, 0x30, 0x20),
            new Color(0x41, 0x38, 0x20),
            new Color(0x41, 0x41, 0x20),
            new Color(0x38, 0x41, 0x20),
            new Color(0x30, 0x41, 0x20),
            new Color(0x28, 0x41, 0x20),
            new Color(0x20, 0x41, 0x20),
            new Color(0x20, 0x41, 0x28),
            new Color(0x20, 0x41, 0x30),
            new Color(0x20, 0x41, 0x38),
            new Color(0x20, 0x41, 0x41),
            new Color(0x20, 0x38, 0x41),
            new Color(0x20, 0x30, 0x41),
            new Color(0x20, 0x28, 0x41),
            new Color(0x2c, 0x2c, 0x41),
            new Color(0x30, 0x2c, 0x41),
            new Color(0x34, 0x2c, 0x41),
            new Color(0x3c, 0x2c, 0x41),
            new Color(0x41, 0x2c, 0x41),
            new Color(0x41, 0x2c, 0x3c),
            new Color(0x41, 0x2c, 0x34),
            new Color(0x41, 0x2c, 0x30),
            new Color(0x41, 0x2c, 0x2c),
            new Color(0x41, 0x30, 0x2c),
            new Color(0x41, 0x34, 0x2c),
            new Color(0x41, 0x3c, 0x2c),
            new Color(0x41, 0x41, 0x2c),
            new Color(0x3c, 0x41, 0x2c),
            new Color(0x34, 0x41, 0x2c),
            new Color(0x30, 0x41, 0x2c),
            new Color(0x2c, 0x41, 0x2c),
            new Color(0x2c, 0x41, 0x30),
            new Color(0x2c, 0x41, 0x34),
            new Color(0x2c, 0x41, 0x3c),
            new Color(0x2c, 0x41, 0x41),
            new Color(0x2c, 0x3c, 0x41),
            new Color(0x2c, 0x34, 0x41),
            new Color(0x2c, 0x30, 0x41),
            new Color(0x00, 0x00, 0x00),
            new Color(0x00, 0x00, 0x00),
            new Color(0x00, 0x00, 0x00),
            new Color(0x00, 0x00, 0x00),
            new Color(0x00, 0x00, 0x00),
            new Color(0x00, 0x00, 0x00),
            new Color(0x00, 0x00, 0x00),
            new Color(0x00, 0x00, 0x00),
        };
	}
}