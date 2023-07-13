using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGui;
using System.Collections.Generic;

namespace AI_assignment
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatch m_SpriteBatch;
        private ShapeBatcher m_ShapeBatcher;

        private ImGuiRenderer m_GUiRenderer;

        private bool m_Paused = false;

        private List<StandardAgent> m_AllAIAgents = new List<StandardAgent>();
        private List<MazeWall> m_AllMazeWalls;
        private ExplorerAgent m_Explorer;

        public Game1()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            m_ShapeBatcher = new ShapeBatcher(this);

            m_GUiRenderer= new ImGuiRenderer(this).Initialize().RebuildFontAtlas();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);

            LoadAllObjects load = new LoadAllObjects(m_Graphics);
            load.LoadAllSceneObjects();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(!m_Paused) 
            {
                float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000f;

                foreach (StandardAgent agent in m_AllAIAgents)
                {
                    agent.Update(seconds);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            m_ShapeBatcher.Begin();

            foreach (StandardAgent agent in m_AllAIAgents)
            {
                agent.Draw(m_ShapeBatcher);
            }

            m_ShapeBatcher.End();


            m_GUiRenderer.BeginLayout(gameTime);
            ImGui.Begin("Debug Interface");

            /*ImGui.SliderFloat("x", ref *//*object here *//*, 0.0f, GraphicsDevice.Viewport.Width, string.Empty);*/
            /*ImGui.SliderFloat("y", ref *//*object here *//*, 0.0f, GraphicsDevice.Viewport.Height, string.Empty);*/

            if(m_Paused)
            {
                if(ImGui.Button("Start"))
                {
                    m_Paused= false;
                }
            }
            else
            {
                if(ImGui.Button("Pause"))
                {
                    m_Paused= true;
                }
            }
            ImGui.End();
            m_GUiRenderer.EndLayout();
        }
    }
}