using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace ChessLG
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Tablero tablero;
        Ficha ficha;
        bool finJuego = false;
        bool turnoAnterior = Ficha.NEGRA;
        bool mueven = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "ChessLG";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1024; // 800 X
            graphics.PreferredBackBufferHeight = 1024; // 600 Y
            //aplicar cambios
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texturas.CASILLA_NEGRA = Texture2D.FromFile(graphics.GraphicsDevice, "negra.png");
            Texturas.CASILLA_BLANCA = Texture2D.FromFile(graphics.GraphicsDevice, "blanca.png");
            Texturas.NEGRO_PEON = Texture2D.FromFile(graphics.GraphicsDevice, "npeon.png");
            Texturas.NEGRO_TORRE = Texture2D.FromFile(graphics.GraphicsDevice, "ntorre.png");
            Texturas.NEGRO_CABALLO = Texture2D.FromFile(graphics.GraphicsDevice, "ncaballo.png");
            Texturas.NEGRO_ALFIL = Texture2D.FromFile(graphics.GraphicsDevice, "nalfil.png");
            Texturas.NEGRO_REINA = Texture2D.FromFile(graphics.GraphicsDevice, "nreina.png");
            Texturas.NEGRO_REY = Texture2D.FromFile(graphics.GraphicsDevice, "nrey.png");
            Texturas.BLANCO_PEON = Texture2D.FromFile(graphics.GraphicsDevice, "bpeon.png");
            Texturas.BLANCO_TORRE = Texture2D.FromFile(graphics.GraphicsDevice, "btorre.png");
            Texturas.BLANCO_CABALLO = Texture2D.FromFile(graphics.GraphicsDevice, "bcaballo.png");
            Texturas.BLANCO_ALFIL = Texture2D.FromFile(graphics.GraphicsDevice, "balfil.png");
            Texturas.BLANCO_REINA = Texture2D.FromFile(graphics.GraphicsDevice, "breina.png");
            Texturas.BLANCO_REY = Texture2D.FromFile(graphics.GraphicsDevice, "brey.png");
            Texturas.RATON = Texture2D.FromFile(graphics.GraphicsDevice, "raton.png");

            tablero = new Tablero();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back 
                == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            if (!finJuego && mueven)
            {
                Window.Title = "ChessLG";

                if (tablero.ahogado(tablero.turno))
                {
                    //Console.Beep();
                    //this.Exit();
                    MessageBox.Show("TABLAS!!");
                    finJuego = true;
                    tablero.movimiento += NotAlg.TABLAS;
                }

                if (tablero.esJaque(tablero.turno, false))
                {
                    Window.Title += " - Jaque!";
                    tablero.movimiento += NotAlg.JAQUE;
                }

                if (tablero.esJaque(tablero.turno, true))
                {
                    //Console.Beep();
                    //Console.Beep();
                    MessageBox.Show("JAQUE MATE!!");
                    //this.Exit();
                    finJuego = true;
                    tablero.movimiento += NotAlg.JAQUE;
                }

                Window.Title += " - " + tablero.movimiento;

                // Añadimos el movimiento
                if (tablero.turno == Ficha.BLANCA || finJuego)
                {
                    tablero.historial.Add(tablero.movimiento);
                    tablero.movimiento = "";
                }

                mueven = false;
            }

            if (turnoAnterior == tablero.turno)
            {
                turnoAnterior = !tablero.turno;
                mueven = true;
            }

            // Prueba heavy de estado
            //Estado status = tablero.getEstado();
            //tablero.setEstado(status);

            // TODO Opciones de fin de juego - Menu, repetir, salir

            // Actualiza la posicion del raton
            mouseFunction();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            // TODO: Add your drawing code here
            tablero.Draw(spriteBatch);

            // Pintar Raton
            spriteBatch.Draw(Texturas.RATON, posRaton, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

         // Ratón
        MouseState mouseState;
        Vector2 posRaton = new Vector2(0,0);
        ArrayList selBlock = new ArrayList();
        void mouseFunction()
        {
            mouseState = Mouse.GetState();

            Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
            posRaton = pos;

            if (!finJuego)
            {
                if (mouseState.LeftButton 
                    == Microsoft.Xna.Framework.Input.ButtonState.Pressed && selBlock.Count == 0)
                {
                    ficha = tablero.click(pos.X, pos.Y);

                    selBlock.Add(ficha);
                }

                if (mouseState.LeftButton 
                    == Microsoft.Xna.Framework.Input.ButtonState.Released && selBlock.Count > 0)
                {
                    selBlock.Clear();
                }
            }
        }
    }
}
