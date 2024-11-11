namespace ConsoleGameEngine
{

    using System;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Abstract class to aid in Gamemaking.
    /// Implements an instance of the ConsoleEngine and has Looping methods.
    /// </summary>
    public class ConsoleGame
    {
        /// <summary> Instance of a ConsoleEngine. </summary>
        public ConsoleEngine Engine { get; private set; }

        /// <summary> A counter representing the current unique frame we're at. </summary>
        public int FrameCounter { get; set; }

        /// <summary> A counter representing the total frames since launch</summary>
        public int FrameTotal { get; private set; }
        /// <summary> Factor for generating framerate-independent physics. time between last frame and current. </summary>
        public float DeltaTime { get; set; }

        /// <summary>The time the program started in DateTime, set after Create()</summary>
        public DateTime StartTime { get; private set; }

        /// <summary> The framerate the engine is trying to run at. </summary>
        public int TargetFramerate { get; set; }

        public bool Running { get; set; }

        private double[] framerateSamples;

        public bool IsRendering = false;

        /// <summary> Initializes the ConsoleGame. Creates the instance of a ConsoleEngine and starts the game loop. </summary>
        /// <param name="width">Width of the window.</param>
        /// <param name="height">Height of the window.</param>
        /// <param name="fontW">Width of the font.</param>
        /// <param name="fontH">´Height of the font.</param>
        /// <param name="m">Framerate mode to run at.</param>
        /// <see cref="FramerateMode"/> <see cref="ConsoleEngine"/>
        public void Construct(int width, int height, int fontW, int fontH, FramerateMode m, Color[] VGAcolors)
        {
            Engine = new ConsoleEngine(width, height, fontW, fontH, VGAcolors);
            StartTime = DateTime.Now;

            // gör special checks som ska gå utanför spelloopen
            // om spel-loopen hänger sig ska man fortfarande kunna avsluta
            while (Running)
            {
            }
        }

        /// <summary> Gets the current framerate the application is running at. </summary>
        /// <returns> Application Framerate. </returns>
        public double GetFramerate()
        {
            return 1 / (framerateSamples.Sum() / (TargetFramerate));
        }


        public void SetTitle(string title)
        {
            NativeMethods.SetConsoleTitle(title);
        }

        public void Exit()
        {
            if (Running)
            {
                Engine.Close();
                Running = false;
            }
        }

        public virtual void Close()
        {
            Exit();
        }
    }
}