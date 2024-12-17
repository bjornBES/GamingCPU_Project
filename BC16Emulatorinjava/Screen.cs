using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using BC16CPUEmulator;
using System;
using System.Timers;
using System.Threading;

namespace BC16CPUEmulator
{
    public class Screen
    {
        GameWindow _Window;
        Shader _shader;
        
        const int Width = 800;
        const int Height = 600;

        float[] m_pointBuffer = new float[Width * Height * 6];

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        public void Instantiate()
        {
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(800, 500),
                Title = "Test",
                Flags = ContextFlags.ForwardCompatible,
                IsEventDriven = true,
                
            };

            GameWindowSettings gameWindowSettings = new GameWindowSettings()
            {
                UpdateFrequency = 30,
            };

            _Window = new GameWindow(gameWindowSettings, nativeWindowSettings);
            _Window.UpdateFrame += UpdateScreen;
            _Window.Load += Start;
            _Window.RenderFrame += RenderFrame;
            _Window.Unload += Unload;

            m_pointBuffer.Initialize();

            _Window.Run();
        }

        private void Unload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.Handle);
        }

        private void RenderFrame(FrameEventArgs obj)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();

            GL.BindVertexArray(_vertexArrayObject);

            GL.DrawArrays(PrimitiveType.Points, 0, m_pointBuffer.Length);

            _Window.SwapBuffers();

            Thread.Sleep(500);
        }

        public Vector3 PixelToScreen(Vector3 pixelCoord)
        {
            return new Vector3((float)(pixelCoord.X / Width), (float)(pixelCoord.Y / Height), 0) * 2 - Vector3.One;
        }

        private void Start()
        {
            GL.ClearColor(1f, 0f, 0f, 1.0f);

            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, m_pointBuffer.Length * sizeof(float), m_pointBuffer, BufferUsageHint.DynamicDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            _shader = new Shader("./BC16Emulator/Shaders/shader.vert", "./BC16Emulator/Shaders/shader.frag");
            _shader.Use();

            int index = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Vector3 position = PixelToScreen(new Vector3i(x, y, 0));
                    m_pointBuffer[index] = position.X;
                    m_pointBuffer[index + 1] = position.Y;
                    m_pointBuffer[index + 2] = position.Z;
                    m_pointBuffer[index + 3] = 0 / byte.MaxValue;
                    m_pointBuffer[index + 4] = 0 / byte.MaxValue;
                    m_pointBuffer[index + 5] = 0 / byte.MaxValue;

                    index += 6;
                }
            }
        }

        double time = 0;
        private void UpdateScreen(FrameEventArgs obj)
        {
            _Window.Title = $"{1000f*obj.Time} Elapsed time {obj.Time * 1000f - time* 1000f}";

            if (_Window.KeyboardState.IsKeyDown(Keys.Escape))
            {
                _Window.Close();
            }
            time = obj.Time;
        }
    }
}
