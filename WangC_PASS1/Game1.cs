using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WangC_PASS1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // screen dimensions
        const int SCREEN_WIDTH = 576;
        const int SCREEN_HEIGHT = 1024;

        // game states
        const int STATS_STATE = 0;
        const int TITLE_STATE = 1;
        const int PRE_GAME_PLAY_STATE = 2;
        const int GAME_PLAY_STATE = 3;
        const int PRE_GAME_OVER_STATE = 4;
        const int GAME_OVER_STATE = 5;

        // button indexes and amount
        const int BTN_AMOUNT = 3;
        const int START_BTN = 0;
        const int STATS_BTN = 1;
        const int MENU_BTN = 2;

        // y value of the title height
        const int TITLE_Y = 200;

        // value of the spacer between title and bird
        const int TITLE_SPACER = 20;

        // title wave constants
        const int TITLE_WAVE_TIME = 250; // time per one oscillation of the title/bird on the title screen
        const int TITLE_WAVE_DISPL = 10; // amplitude (horizontal displace) from TITLE_Y

        // button y value on screen
        const int BTN_Y = 650;

        // amount of ground images used
        const int GRD_AMOUNT = 2;

        // ground y position
        const int GRD_Y = 800;

        // bird should be at 130 during pre game play and game play
        const int BIRD_X = 130;

        // background scroll speed
        const float SCRL_SPD = 3.5f;

        // bird animation duration in millisecond
        const int BIRD_ANIM_DUR_TITLE = 400; // time during title screen
        const int BIRD_ANIM_DUR_GAME = 200; // time during game screen

        // fade time and opacity constants
        const int FADE_STATE_TIME = 500; // 1500 ms to fade out and 1500 to fade out
        const int FADE_DEATH_TIME = 100;
        const float FULL_OPACITY = 1f;
        const float EMPTY_OPACITY = 0f;

        // score variables
        const int SCOR_RST = 0; // score reset position

        // horizontal and vertical pipe separation
        const int PIPE_X_SPC = 200;
        const int PIPE_Y_SPC = 210;

        // UNDONE
        const int PIPE_MAX_Y = 0;
        const int PIPE_MIN_Y_FACTR = 88;
        const int PIPE_COUNT = 3;
        const int PIPE_RST_X = 600;

        // top and bottom values
        const int TOP = 0;
        const int BOT = 1;

        // max and min speeds of the bird
        const int MAX_SPD = 15;
        const int MIN_SPD = -10;

        // the speed of gravity and flaps
        const float GRAVITY = 0.5f;
        const int FLAP = -10;

        // y location of the score counter in pre/game play
        const int CUR_SCR_Y = 50;

        // spacer between large numbers
        const int NUM_SPC_L = 5;

        // length of death timer
        const int DEATH_TIMER = 500;

        // type of death
        const int GRD_DEATH = 1;
        const int PIPE_DEATH = 2;

        // spacer between game over title and result box
        const int GAME_OVER_RSLT_SPC_Y = 10;

        // create a instance of the random function
        Random rand = new Random();

        // initialize game state variable, and set to TITLE SCREEN
        int gameState = TITLE_STATE;
        int tempGameState = TITLE_STATE; // used when fading between sates

        // initialize mouse variables
        MouseState mouse;
        MouseState prevMouse;

        // initialize keyboard variables
        KeyboardState kb;
        KeyboardState prevKb;

        // initialize default font
        SpriteFont debugFont;

        // initialize background variables
        Texture2D bgImg;
        Vector2 bgPos;

        // initialize title variables
        Texture2D titleImg;
        Vector2 titlePos;
        Vector2 titlePosRst;

        // initialize bird variables
        Texture2D birdAnimImg;
        Animation birdAnim;
        Vector2 birdPos;
        float birdSpd;
        Vector2 birdTitlePosRst; // reset position of the bird on title screen
        Vector2 birdPrePosRst = new Vector2(BIRD_X, GRD_Y / 2); // reset position of the bird on the pre game screen 

        // bird death variables
        Timer birdDeathTimer = new Timer(DEATH_TIMER, false);
        bool isBirdDead = false;
        int birdDeathType;

        // count flaps
        int flapCnt;

        // initialize button variables
        Texture2D[] btnImgs = new Texture2D[BTN_AMOUNT];
        Rectangle[] btnRecs = new Rectangle[BTN_AMOUNT];

        // initialize ground variables
        Texture2D grdImg;
        Vector2[] grdPoss = new Vector2[GRD_AMOUNT];
        Rectangle[] grdRecs = new Rectangle[GRD_AMOUNT];

        // initialize fader variables
        Texture2D fadeImg;
        Rectangle scrnRec = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT); // rectangle of fade is size of screen
        Timer fadeCurTime = new Timer(FADE_STATE_TIME, false); // set to fade statetime for now
        float fadeOpacity = FULL_OPACITY; // fade opacity, set the opacity to 100

        // flash fade flag, tells weather if the fade is going in or out
        bool flashFadeIn = false;

        // initialize click instructions variables
        Texture2D intrucImg;
        Vector2 intrcuPos;

        // initialize get ready title variables
        Texture2D readyTitleImg;
        Vector2 readyTitlePos;

        // initialize score variables
        int curScore = SCOR_RST;
        Vector2 curScorPos = new Vector2(SCREEN_WIDTH / 2, CUR_SCR_Y);
        Texture2D numsLrgImg;
        Texture2D numsSmlImg;

        // initialize pipe variables
        Texture2D[] pipeImgs = new Texture2D[2]; // only 2 (top and bottom)
        Rectangle[,] pipeRecs = new Rectangle[PIPE_COUNT, PIPE_COUNT];
        Vector2[,] pipePos = new Vector2[PIPE_COUNT, PIPE_COUNT];
        bool[] pipesPass = new bool[PIPE_COUNT];

        // initialize game over title variables
        Texture2D gameOvrImg;
        Vector2 gameOvrPos;
        Vector2 gameOvrPosRst;
        Vector2 gameOvrPosFin;

        // initialize game over title variables
        Texture2D rsltsBoxImg;
        Vector2 rsltBoxPos;
        Vector2 rsltBoxPosRst;
        Vector2 rsltBoxPosFin;
        Vector2 rsltCurScorPos;
        Vector2 rsltHighScorPos;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;

            graphics.ApplyChanges();

            IsMouseVisible = true;

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

            // load debug font
            debugFont = Content.Load<SpriteFont>("Fonts/DebugFont");

            // load background image and initialize position
            bgImg = Content.Load<Texture2D>("Images/Backgrounds/Background");
            bgPos = Vector2.Zero;

            // load title image
            titleImg = Content.Load<Texture2D>("Images/Sprites/Title");

            // load bird animation sheet and animation
            birdAnimImg = Content.Load<Texture2D>("Images/Sprites/Bird");
            birdAnim = new Animation(birdAnimImg, 4, 1, 4, 0, 1, Animation.ANIMATE_FOREVER, BIRD_ANIM_DUR_TITLE, Vector2.Zero, true); // set animation with vector since for now, since we need to width of the bird

            // store the reset title position & bird position and rectangle
            titlePosRst = new Vector2((SCREEN_WIDTH - titleImg.Width - birdAnim.GetDestRec().Width - TITLE_SPACER) / 2, TITLE_Y);
            birdTitlePosRst = new Vector2(titlePosRst.X + titleImg.Width + TITLE_SPACER, TITLE_Y + titleImg.Height / 2 - birdAnim.GetDestRec().Height / 2);
            //Console.WriteLine(birdTitlePosRst.ToString());

            // set regular title and bird positions to reset positions
            titlePos = titlePosRst;
            birdPos = birdTitlePosRst;

            // translate animation to (reset) position
            birdAnim.TranslateTo(birdPos.X, birdPos.Y);

            // load button images
            btnImgs[START_BTN] = Content.Load<Texture2D>("Images/Sprites/StartBtn");
            btnImgs[STATS_BTN] = Content.Load<Texture2D>("Images/Sprites/StatsBtn");
            btnImgs[MENU_BTN] = Content.Load<Texture2D>("Images/Sprites/MenuBtn");

            // store button rectangles
            // the start and stats button should be such that the remaining space is evenly spaced in thirds
            btnRecs[START_BTN] = new Rectangle((SCREEN_WIDTH - 2 * btnImgs[START_BTN].Width) / 3, BTN_Y, btnImgs[START_BTN].Width, btnImgs[START_BTN].Height);
            btnRecs[STATS_BTN] = new Rectangle(SCREEN_WIDTH - ((SCREEN_WIDTH - 2 * btnImgs[STATS_BTN].Width) / 3) - btnImgs[STATS_BTN].Width, BTN_Y, btnImgs[STATS_BTN].Width, btnImgs[STATS_BTN].Height);
            btnRecs[MENU_BTN] = new Rectangle((SCREEN_WIDTH / 2) - btnImgs[MENU_BTN].Width / 2, BTN_Y, btnImgs[MENU_BTN].Width, btnImgs[MENU_BTN].Height);

            // store ground image, position, and rectangle variables data
            grdImg = Content.Load<Texture2D>("Images/Backgrounds/Ground");
            grdPoss[0] = new Vector2(0, GRD_Y);
            grdRecs[0] = new Rectangle((int)grdPoss[0].X, (int)grdPoss[0].Y, grdImg.Width, grdImg.Height);
            grdPoss[1] = new Vector2(grdImg.Width, GRD_Y);
            grdRecs[1] = new Rectangle((int)grdPoss[1].X, (int)grdPoss[1].Y, grdImg.Width, grdImg.Height);

            // load fader image
            fadeImg = Content.Load<Texture2D>("Images/Sprites/Fader");

            // load click instruction image and store it's position
            intrucImg = Content.Load<Texture2D>("Images/Sprites/ClickInstruction");
            intrcuPos = new Vector2(SCREEN_WIDTH / 2 - intrucImg.Width / 2, GRD_Y / 2); 

            // load click instruction image and store it's position
            readyTitleImg = Content.Load<Texture2D>("Images/Sprites/GetReady");
            readyTitlePos = new Vector2(SCREEN_WIDTH / 2 - readyTitleImg.Width / 2, SCREEN_HEIGHT / 3 - readyTitleImg.Height / 2); // horizontally centered and 1/3 of the way down from top to ground

            // load large and small numbers images
            numsLrgImg = Content.Load<Texture2D>("Images/Sprites/BigNums");
            numsSmlImg = Content.Load<Texture2D>("Images/Sprites/SmallNums");

            // load pip image and store other pipe data
            pipeImgs[TOP] = Content.Load<Texture2D>("Images/Sprites/TopPipe");
            pipeImgs[BOT] = Content.Load<Texture2D>("Images/Sprites/BottomPipe");

            // initialize the rectangles of the pipes
            for (int i = 0; i < PIPE_COUNT; i++)
            {
                pipeRecs[i, TOP] = new Rectangle(0, 0, pipeImgs[TOP].Width, pipeImgs[TOP].Height);

                pipeRecs[i, BOT] = new Rectangle(0, 0, pipeImgs[BOT].Width, pipeImgs[BOT].Height);
            }

            ResetPipes();

            // load game over title image
            gameOvrImg = Content.Load<Texture2D>("Images/Sprites/GameOver");
            gameOvrPosFin = new Vector2(SCREEN_WIDTH / 2 - gameOvrImg.Width / 2, SCREEN_HEIGHT / 3 - gameOvrImg.Height / 2);
            gameOvrPos = gameOvrPosFin;

            // load result box images
            rsltsBoxImg = Content.Load<Texture2D>("Images/Sprites/ReusultsBox");
            rsltBoxPosFin = new Vector2(SCREEN_WIDTH / 2 - rsltsBoxImg.Width / 2, gameOvrPos.Y - gameOvrImg.Height - GAME_OVER_RSLT_SPC_Y);
            rsltBoxPos = rsltBoxPosFin;


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // update mouse variables
            prevMouse = mouse;
            mouse = Mouse.GetState();

            // update keyboard variables
            prevKb = kb;
            kb = Keyboard.GetState();

            // TODO: Add your update logic here
            switch (gameState)
            {
                case STATS_STATE:

                    break;
                case TITLE_STATE:
                    // update title screen
                    UpdateTitle(gameTime);
                    break;
                case PRE_GAME_PLAY_STATE:
                    // update pre game play screen
                    UpdatePreGame(gameTime);
                    break;
                case GAME_PLAY_STATE:
                    // Update game play
                    UpdateGamePlay(gameTime);
                    break;
                case PRE_GAME_OVER_STATE:
                    UpdatePreGameOvr(gameTime);
                    break;
                case GAME_OVER_STATE:
                    UpdateGameOvr(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            spriteBatch.Begin();

            // draw the background 
            spriteBatch.Draw(bgImg, bgPos, Color.White);

            switch (gameState)
            {
                case STATS_STATE:
                    //draw stats screen
                    break;

                case TITLE_STATE:
                    // draw title screen
                    DrawTitleScreen(gameTime);
                    break;

                case PRE_GAME_PLAY_STATE:
                    // draw pre game play state
                    DrawPreGamePlay(gameTime);
                    break;

                case GAME_PLAY_STATE:
                    // draw game play state
                    DrawGamePlay(gameTime);
                    break;

                case PRE_GAME_OVER_STATE:
                    DrawPreGameOvr(gameTime);
                    break;

                case GAME_OVER_STATE:
                    DrawGameOvr(gameTime);
                    break;
            }



            DrawMouseDebug(spriteBatch, mouse, debugFont);
            DrawHitBox(spriteBatch, fadeImg, birdAnim.GetDestRec(), Color.Red, 0.3f);
            //DrawHitBox(spriteBatch, fadeImg, new Rectangle((int)titlePos.X, (int)titlePos.Y, titleImg.Width, titleImg.Height), Color.Blue);
            //Console.WriteLine("Time passed: " + fadeCurTime.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL) + " Opacity: " + fadeOpacity);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void UpdateTitle(GameTime gameTime)
        {
            // update bird animation
            birdAnim.Update(gameTime);

            // move title and bird up and down
            titlePos.Y = TITLE_WAVE_DISPL * (float)(Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / TITLE_WAVE_TIME) * Math.PI) + titlePosRst.Y;
            birdPos.Y = TITLE_WAVE_DISPL * (float)(Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / TITLE_WAVE_TIME) * Math.PI) + birdTitlePosRst.Y;
            birdAnim.TranslateTo(birdPos.X, birdPos.Y);

            // update ground scrolling
            UpdateGround();

            // check if there is no fade
            if (fadeCurTime.IsInactive())
            {
                // check if player clicks the start button
                if (btnRecs[START_BTN].Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    ResetFade(FADE_STATE_TIME, true, EMPTY_OPACITY);

                    tempGameState = PRE_GAME_PLAY_STATE;
                }
                if (btnRecs[STATS_BTN].Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    ResetFade(FADE_STATE_TIME, true, EMPTY_OPACITY);

                    tempGameState = STATS_STATE;
                }
            }
            
            // if fade timer is on update it
            else if (fadeCurTime.IsActive())
            {
                // update fade time
                UpdateFade(gameTime, FADE_STATE_TIME, false);
            }
            // if fade timer is completed to switch and prepare pre gameplay
            else if (fadeCurTime.IsFinished())
            {
                switch (tempGameState)
                {
                    case PRE_GAME_PLAY_STATE:
                        gameState = tempGameState;

                        // set bird to pre game position
                        birdPos.X = birdPrePosRst.X;
                        birdPos.Y = birdPrePosRst.Y;
                        birdAnim.TranslateTo(birdPos.X, birdPos.Y);

                        // change game state to stats
                        gameState = tempGameState;

                        // reset current score varibles
                        ResetScore();
                        break;

                    case STATS_STATE:


                        break;
                }
                
                //// start and reset fade timer
                //fadeCurTime.ResetTimer(true);
                //fadeOpacity = FULL_OPACITY;

                ResetFade(FADE_STATE_TIME, true, FULL_OPACITY);
               
            }
        }

        // draw the title screen
        private void DrawTitleScreen(GameTime gameTime)
        {
            DrawGround();
            
            // draw the title
            spriteBatch.Draw(titleImg, titlePos, Color.White);

            // draw animated bird
            birdAnim.Draw(spriteBatch, Color.White);

            // draw the starting and stats button in the title screen
            spriteBatch.Draw(btnImgs[START_BTN], btnRecs[START_BTN], Color.White);
            spriteBatch.Draw(btnImgs[STATS_BTN], btnRecs[STATS_BTN], Color.White);

            if (fadeCurTime.IsActive() || (fadeCurTime.IsFinished() && fadeOpacity == FULL_OPACITY)) DrawFadeScr(Color.Black);
        }

        // update the pre game play screen
        private void UpdatePreGame(GameTime gameTime)
        {            
            // update the bird animation
            birdAnim.Update(gameTime);

            // update ground scrolling
            UpdateGround();
            
            // only update the fade if the timer is active
            if (fadeCurTime.IsActive())
            {
                UpdateFade(gameTime, FADE_STATE_TIME, true);
            }

            //  other wise check if the player has pressed space or clicked to start
            else if (fadeCurTime.IsInactive() && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed || (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space)))
            {
                // change the game state to game play
                gameState = GAME_PLAY_STATE;

                // reset the positions of pipes
                ResetPipes();

                // change the duration of the bird animation
                birdAnim.SetDuration(BIRD_ANIM_DUR_GAME);

                // reset flap count && bird death flag
                flapCnt = 0;
                isBirdDead = false;
            }
        }

        // draw the pre game play screen
        private void DrawPreGamePlay(GameTime gameTime)
        {
            // draw the bird
            birdAnim.Draw(spriteBatch, Color.White);

            DrawGround();

            // draw click instructions
            spriteBatch.Draw(intrucImg, intrcuPos, Color.White);

            // draw get ready title
            spriteBatch.Draw(readyTitleImg, readyTitlePos, Color.White);

            // check if the timer is still running, if so draw the fade
            if (fadeCurTime.IsActive() || (fadeCurTime.IsFinished() && fadeOpacity == FULL_OPACITY)) DrawFadeScr(Color.Black);
            // otherwise reset the fade variables
            else ResetFade(FADE_DEATH_TIME, false, EMPTY_OPACITY); // set it to death fade time since the player is going to die before game over screen

            DrawNumberSequence(curScore.ToString(), numsLrgImg, curScorPos, NUM_SPC_L);
        }

        //update the current game play
        private void UpdateGamePlay(GameTime gameTime)
        {
            // update the birds animation
            birdAnim.Update(gameTime);
            Console.WriteLine(birdAnim.IsAnimating().ToString());

            UpdatePipes(gameTime);

            UpdateBirdPos(gameTime);

            UpdateGround();
        }

        private void DrawGamePlay(GameTime gameTime)
        {
            DrawPipes();

            DrawGround();

            birdAnim.Draw(spriteBatch, Color.White);

            DrawScore();
        }

        private void UpdatePreGameOvr(GameTime gameTime)
        {
            UpdateFadeDeath(gameTime);

            switch (birdDeathType)
            {
                case GRD_DEATH:
                    birdDeathTimer.Update(gameTime);
                    break;
                case PIPE_DEATH:
                    UpdateBirdPos(gameTime);
                    break;
            }


            // change to game over state after death timer finish
            if (birdDeathTimer.IsFinished()) gameState = GAME_OVER_STATE;
        }

        private void DrawPreGameOvr(GameTime gameTime)
        {
            DrawPipes();

            DrawGround();

            birdAnim.Draw(spriteBatch, Color.White);
            Console.WriteLine(birdAnim.IsAnimating().ToString());
            Console.WriteLine(birdAnim.GetCurFrame().ToString());

            DrawFadeScr(Color.White);

            //DEBUG
            DrawDeathTimer(spriteBatch);
        }

        private void UpdateGameOvr(GameTime gameTime)
        {

        }


        private void DrawGameOvr(GameTime gameTime)
        {
            DrawPipes();

            DrawGround();

            birdAnim.Draw(spriteBatch, Color.White);

            spriteBatch.Draw(gameOvrImg, gameOvrPos, Color.White);
        }

        private void UpdateBirdPos(GameTime gameTime)
        {
            if (!isBirdDead)
            {
                if ((mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Released) || (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space)))
                {
                    birdSpd = FLAP;

                    flapCnt++;
                }
            }

            birdSpd += GRAVITY;

            birdSpd = MathHelper.Clamp(birdSpd, MIN_SPD, MAX_SPD);

            birdPos.Y += birdSpd;

            birdAnim.TranslateTo(birdPos.X, birdPos.Y);

            //BirdCollision(gameTime);
        }

        private void BirdCollision(GameTime gameTime)
        {
            // check if bird has died, if it has not need to check if it hits the pipes
            if (!isBirdDead)
            {
                for (int i = 0; i < PIPE_COUNT; i++)
                {
                    if (pipeRecs[i, TOP].Intersects(birdAnim.GetDestRec()) || pipeRecs[i, BOT].Intersects(birdAnim.GetDestRec()))
                    {
                        // store the death type
                        birdDeathType = PIPE_DEATH;

                        // change the game state
                        gameState = PRE_GAME_OVER_STATE;

                        DeathDelayReset(gameTime, birdDeathType);
                        break;
                    }
                }
            }
            

            // check if collision with ground
            if (birdAnim.GetDestRec().Bottom > GRD_Y)
            {
                birdAnim.TranslateTo(birdPos.X, GRD_Y - birdAnim.GetDestRec().Height);

                // store the death type
                birdDeathType = GRD_DEATH;

                // change the game state
                gameState = PRE_GAME_OVER_STATE;

                // only reset death if bird didn't die yet
                // it could be possible if bird died from pipe before it hit ground
                if (!isBirdDead) DeathDelayReset(gameTime, birdDeathType);
                else birdDeathTimer.Activate();
            }
        }

        private void DeathDelayReset(GameTime gameTime, int deathType)
        {
            // deactivate the bird's animation and set idle frame to 1
            birdAnim.Deactivate();
            birdAnim.SetIdleFrame(1); // NOTE NEED TO FIRST DEACTIVE then set idle frame. 


            birdDeathTimer.ResetTimer(false);

            ResetFade(FADE_DEATH_TIME, true, EMPTY_OPACITY);

            gameState = PRE_GAME_OVER_STATE;

            isBirdDead = true;

            if (deathType == GRD_DEATH) birdDeathTimer.Activate();
        }

        private void UpdateFadeDeath(GameTime gameTime)
        {
            if (flashFadeIn)
            {
                UpdateFade(gameTime, FADE_DEATH_TIME, false);
            }
            else
            {
                UpdateFade(gameTime, FADE_DEATH_TIME, true);
            }
        }

        // updates the fade time, and returns true if fade timer is completed
        // if isIn is true, then it's fading in, otherwise it's fading out
        private void UpdateFade(GameTime gameTime, int fadeTime, bool isIn)
        {
            fadeCurTime.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            if (isIn)
            {
                // store the opacity of the fade, as it should be
                fadeOpacity = Math.Min(1f, (float)fadeCurTime.GetTimeRemaining() / fadeTime * FULL_OPACITY);
            }
            else
            {
                fadeOpacity = Math.Min(1f, (FADE_STATE_TIME - (float)fadeCurTime.GetTimeRemaining()) / fadeTime) * FULL_OPACITY;
            }
        }

        // draw the fade screen
        // if isIn is true, then it's fading in, otherwise it's fading out
        private void DrawFadeScr(Color color)
        {
            spriteBatch.Draw(fadeImg, scrnRec, color * fadeOpacity);
        }

        // reset all fade variables
        private void ResetFade(int fadeTime, bool isActive, float startOpacity)
        {
            fadeOpacity = startOpacity;
            fadeCurTime.SetTargetTime(fadeTime);
            fadeCurTime.ResetTimer(isActive);

            if (fadeTime == FADE_DEATH_TIME)
            {
                flashFadeIn = false;
            }
        }

        // update the scrolling of the ground images
        private void UpdateGround()
        {
            // update ground scrolling
            for (int i = 0; i < grdRecs.Length; i++)
            {
                // check if the right side of the ground rectangle has passed/reach the screen's right (0) 
                if (grdRecs[i].Right <= 0)
                {
                    // reposition image back to the other end of the other image
                    grdPoss[i].X = grdImg.Width;
                }
                // move each ground image left by the scroll speed
                grdPoss[i].X -= SCRL_SPD;

                // update ground rectangles to match ground position
                grdRecs[i].X = (int)grdPoss[i].X;
            }
        }

        // draw the ground
        private void DrawGround()
        {
            // draw the ground
            spriteBatch.Draw(grdImg, grdRecs[0], Color.White);
            spriteBatch.Draw(grdImg, grdRecs[1], Color.White);
        }

        // update pipes
        private void UpdatePipes(GameTime gameTime)
        {
            for (int i = 0; i < PIPE_COUNT; i++)
            {
                // move the pipes by the scroll speed to the left
                pipePos[i, TOP].X -= SCRL_SPD;
                pipePos[i, BOT].X -= SCRL_SPD;


                // check if the right side of the ground rectangle has passed/reach the screen's right (0) 
                if (pipeRecs[i, TOP].Right <= 0)
                {
                    GeneratePipe(i);
                }
                else if (!pipesPass[i] && pipeRecs[i, TOP].Right <= birdAnim.GetDestRec().Left)
                {
                    UpdateScore(gameTime);
                    pipesPass[i] = true;
                }

                pipeRecs[i, TOP].X = (int)pipePos[i, TOP].X;
                pipeRecs[i, BOT].X = (int)pipePos[i, BOT].X;
            }
        }

        private void GeneratePipe(int i)
        {
            // reset pip passed
            pipesPass[i] = false;

            // the right most pipe would be one index smaller
            int rightMost = i - 1;
            if (i == 0) rightMost = PIPE_COUNT - 1;
            //else rightMost = i - 1;

            // reposition image back to the other end of the other image
            pipePos[i, TOP].X = pipePos[rightMost, TOP].X + pipeRecs[rightMost, TOP].Width + PIPE_X_SPC;
            pipePos[i, BOT].X = pipePos[rightMost, BOT].X + pipeRecs[rightMost, BOT].Width + PIPE_X_SPC;

            // vertical displacement
            int verDis = rand.Next(-400, 400);

            pipePos[i, TOP].Y = MathHelper.Clamp(pipePos[rightMost, TOP].Y + verDis, -1 * pipeRecs[rightMost, TOP].Height + PIPE_MIN_Y_FACTR, PIPE_MAX_Y);
            pipePos[i, BOT].Y = pipePos[i, TOP].Y + pipeRecs[i, TOP].Height + PIPE_Y_SPC;

            pipeRecs[i, TOP].X = (int)pipePos[i, TOP].X;
            pipeRecs[i, TOP].Y = (int)pipePos[i, TOP].Y;
            pipeRecs[i, BOT].X = (int)pipePos[i, BOT].X;
            pipeRecs[i, BOT].Y = (int)pipePos[i, BOT].Y;
        }

        private void ResetPipes()
        {

            for (int i = 0; i < PIPE_COUNT; i++)
            {
                // reset if pipe has passed bird
                pipesPass[i] = false;

                // locations of the ith pipe image
                // check if i < 0, other wise i is 0
                if (i > 0)
                {

                    // vertical displacement
                    int verDis = rand.Next(-400, 400);

                    // the top of the pipe will be +/- 400 of the previous pipe
                    // but also needs to be between 0, and - pipe height + 88
                    pipePos[i, TOP].Y = MathHelper.Clamp(pipePos[i - 1, TOP].Y + verDis, -1 * pipeRecs[i - 1, TOP].Height + PIPE_MIN_Y_FACTR, PIPE_MAX_Y);
                    pipePos[i, BOT].Y = pipePos[i, TOP].Y + pipeRecs[i, TOP].Height + PIPE_Y_SPC; // 

                    pipePos[i, TOP].X = pipePos[i - 1, TOP].X + pipeRecs[i - 1, TOP].Width + PIPE_X_SPC;
                    pipePos[i, BOT].X = pipePos[i - 1, BOT].X + pipeRecs[i - 1, BOT].Width + PIPE_X_SPC;
                }
                else
                {
                    // the right most will be the always be between 0 and - the negative height of pipe plus '88'
                    pipePos[i, TOP].Y = rand.Next(-1 * pipeRecs[0, TOP].Height + PIPE_MIN_Y_FACTR, PIPE_MAX_Y);
                    pipePos[i, BOT].Y = pipePos[i, TOP].Y + pipeRecs[i, TOP].Height + PIPE_Y_SPC; 

                    pipePos[i, TOP].X = PIPE_RST_X;
                    pipePos[i, BOT].X = PIPE_RST_X;
                }

                pipeRecs[i, TOP].X = (int)pipePos[i, TOP].X;
                pipeRecs[i, BOT].X = (int)pipePos[i, BOT].X;
                pipeRecs[i, TOP].Y = (int)pipePos[i, TOP].Y;
                pipeRecs[i, BOT].Y = (int)pipePos[i, BOT].Y;

            }
        }

        // draw the pipes
        private void DrawPipes()
        {
            for (int i = 0; i < PIPE_COUNT; i++)
            {
                spriteBatch.Draw(pipeImgs[TOP], pipeRecs[i, TOP], Color.White);
                spriteBatch.Draw(pipeImgs[BOT], pipeRecs[i, BOT], Color.White);
            }
        }

        private void ResetScore()
        {
            curScorPos.X = SCREEN_WIDTH / 2;

            curScore = SCOR_RST;
        }

        private void UpdateScore(GameTime gametime, int increment = 1) 
        {
            curScore += increment;

            if (Math.Floor(Math.Log10(curScore)) > Math.Floor(Math.Log10(curScore - increment)) && curScore > 9)
            {    
                curScorPos.X -= numsLrgImg.Width / 10 - NUM_SPC_L;
            }

        }

        // draw the current score
        private void DrawScore()
        {
            DrawNumberSequence(curScore.ToString(), numsLrgImg, curScorPos, NUM_SPC_L);
        }



        // draw numbers with the num texture when given a number in the form a string
        public void DrawNumberSequence(string num, Texture2D texture, Vector2 loc, int numSpacer)
        {
            int digitSize = texture.Width / 10; // 10 digits

            for (int i = 0; i < num.Length; i++)
            {
                int digit = num[i] - '0';

                Vector2 drawLoc = new Vector2(loc.X + i * digitSize + i * numSpacer, loc.Y);
                Rectangle sourceRec = new Rectangle(digit * digitSize, 0, digitSize, texture.Height);

                spriteBatch.Draw(texture, drawLoc, sourceRec, Color.White);
            }
        }

        private void DrawMouseDebug(SpriteBatch spriteBatch, MouseState mouse, SpriteFont debugFont)
        {
            spriteBatch.DrawString(debugFont, mouse.Position.ToString(), new Vector2(mouse.X + 10, mouse.Y + 10), Color.Black);
        }

        private void DrawHitBox(SpriteBatch spriteBatch, Texture2D img, Rectangle rec, Color color, float trans = 0.5f)
        {
            spriteBatch.Draw(img, rec, color * trans);
        }

        private void DrawDeathTimer(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(debugFont, birdDeathTimer.GetTimePassedAsString(Timer.FORMAT_MIN_SEC_MIL), new Vector2(0, 120), Color.Black);
        }

    }
}
