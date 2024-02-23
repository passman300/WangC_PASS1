using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
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
        const int MENU_STATE = 1;
        const int PRE_GAME_PLAY_STATE = 2;
        const int GAME_PLAY_STATE = 3;
        const int PRE_GAME_OVER_STATE = 4;
        const int GAME_OVER_STATE = 5;

        // button indexes and amount
        const int BTN_COUNT = 3;
        const int START_BTN = 0;
        const int STATS_BTN = 1;
        const int MENU_BTN = 2;

        // y value of the title height
        const int TITLE_Y = 200;

        // value of the spacer between title and bird
        const int TITLE_SPACER = 20;

        // title wave constants
        const int TITLE_WAVE_TIME = 500; // time per one half a period of the title/bird on the title screen
        const int TITLE_WAVE_AMP = 10; // amplitude (horizontal displace) from TITLE_Y

        // button y value on screen
        const int BTN_Y = 670;

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
        const int FADE_STATE_TIME = 800; // 1500 ms to fade out and 1500 to fade out
        const int FADE_DEATH_TIME = 200;
        const float FULL_OPACITY = 1f;
        const float EMPTY_OPACITY = 0f;

        // score variables
        const int SCOR_RST = 0; // score reset position

        // x and y positions of stats and average stats
        const int PLAYS_Y = 325;
        const int FLAPS_Y = 375;
        const int AVG_SCOR_Y = 445;
        const int AVG_FLAPS_Y = 495;
        const int STATS_SCOR_X = 350;

        // num of scores counted in stats
        const int TOP_SCORS = 5;

        // x position of top 5 scors
        const int TOP_SCORS_X = 480;
        const int TOP_SCORS_Y = 325;
        const int TOP_SCORS_Y_SPC = 50;

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

        // max and min rotations of the bird
        const int MAX_ANGL = 90;
        const int MIN_ANGL = -30;

        // the speed of gravity and flaps
        const float GRAVITY = 0.5f;
        const int FLAP = -10;

        // y location of the score counter in pre/game play
        const int CUR_SCR_Y = 50;

        // spacer between large numbers
        const int NUM_SPC_FACTOR = 4;

        // length of death timer
        const int DEATH_TIMER = 500;

        // type of death
        const int GRD_DEATH = 1;
        const int PIPE_DEATH = 2;

        // spacer between game over title and result box
        const int GAME_OVR_RSLT_SPC_Y = 10;

        // timer for how long it takes for game over title to fade in (ms)
        const int GAME_OVR_FADE_TIME = 1000;
        const int GAME_OVR_WAVE_AMP = 100;
        const int GAME_OVR_WAVE_DISPL = 216;

        // speed of the result box
        const int RSLT_BOX_SPD = -130;

        // positions of current and best score on result box
        const int RSLT_SCOR_X = 470;
        const int RSLT_CUR_SCOR_Y = 460;
        const int RSLT_HIGH_SCOR_Y = 543; // guess


        // reset and final location of the menu button
        const int MENU_BTN_Y_RST = SCREEN_HEIGHT;
        const int MENU_BTN_Y_FIN = BTN_Y;

        // time it takes for menu to move up (ms)
        const int MENU_POP_UP_TIME = 300;

        // new best tag location
        const int NEW_BEST_X = 326;
        const int NEW_BEST_Y = 505;

        // timer for counting up score in game over screen (ms)
        const int CNT_DELAY = 50;

        // coin count and index
        const int COINS_COUNT = 4;
        const int BRONZE_COIN = 0;
        const int SILVER_COIN = 1;
        const int GOLD_COIN = 2;
        const int PLAT_COIN = 3;

        // scores requirements for their respective scores
        const int BRONZE_SCR = 10;
        const int SILVER_SCR = 20;
        const int GOLD_SCR = 30;
        const int PLAT_SCR = 40;

        // location of the medals realtive to the top of the restults box
        const int COIN_OFFSET_Y = 86;
        const int COIN_OFFSET_X = 53;

        // stats file path, and blank template of stats
        const string STATS_FILE_PATH = "stats.txt";
        const string FILE_TEMPLATE = "0,0\n0";

        // volume of each sound effect
        const float FADE_SND_VOL = 0.25f;
        const float FLAP_SND_VOL = 0.3f;
        const float POINT_SND_VOL = 0.3f;
        const float HIT_SND_VOL = 0.6f;
        const float DIE_SND_VOL = 0.4f;

        // length of spark length
        const int SPARK_DUR = 500;

        // create a instance of the random function
        Random rand = new Random();

        // initialize game state variable, and set to TITLE SCREEN
        int gameState = MENU_STATE;
        int nxtGameState;

        // initialize mouse variables
        MouseState mouse;
        MouseState prevMouse;

        // initialize keyboard variables
        KeyboardState kb;
        KeyboardState prevKb;

        // file IO variables
        StreamReader inFile;
        StreamWriter outFile;

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

        // initialize button variables
        Texture2D[] btnImgs = new Texture2D[BTN_COUNT];
        Rectangle[] btnRecs = new Rectangle[BTN_COUNT];

        // initialize ground variables
        Texture2D grdImg;
        Vector2[] grdPoss = new Vector2[GRD_AMOUNT];
        Rectangle[] grdRecs = new Rectangle[GRD_AMOUNT];

        // initialize fader variables
        Texture2D fadeImg;
        Rectangle scrnRec = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT); // rectangle of fade is size of screen
        Timer fadeCurTime = new Timer(FADE_STATE_TIME, false); // set to fade statetime for now
        float fadeOpacity = FULL_OPACITY; // fade opacity, set the opacity to 100

        // tells weather flashed reached half way
        bool fadedHalf = false;

        // initialize score variables
        int curScor = SCOR_RST;
        int bestScor = SCOR_RST;
        bool isNewBest = false;
        Vector2 curScorPos = new Vector2(SCREEN_WIDTH / 2, CUR_SCR_Y);
        Texture2D numsLrgImg;
        Texture2D numsSmlImg;

        // stat variables
        int playsCnt = 0;
        int flapCnt = 0; // number of total flaps
        int avgScor;
        int avgFlaps;
        List<int> scorsList = new List<int>();

        // initialize positions of each stat on the stats box
        Vector2 playsPos = new Vector2(STATS_SCOR_X, PLAYS_Y);
        Vector2 flapsPos = new Vector2(STATS_SCOR_X, FLAPS_Y);
        Vector2 avgScorsPos = new Vector2(STATS_SCOR_X, AVG_SCOR_Y);
        Vector2 avgFlapsPos = new Vector2(STATS_SCOR_X, AVG_FLAPS_Y);
        Vector2 topScorsPos = new Vector2(TOP_SCORS_X, TOP_SCORS_Y);

        // initialize stats page variables
        Texture2D statsBoxImg;
        Vector2 statsBoxPos;

        // initialize click instructions variables
        Texture2D intrucImg;
        Vector2 intrcuPos;

        // initialize get ready title variables
        Texture2D readyTitleImg;
        Vector2 readyTitlePos;

        // initialize pipe variables
        Texture2D[] pipeImgs = new Texture2D[2]; // only 2 (top and bottom)
        Rectangle[,] pipeRecs = new Rectangle[PIPE_COUNT, PIPE_COUNT];
        Vector2[,] pipePos = new Vector2[PIPE_COUNT, PIPE_COUNT];
        bool[] pipesPass = new bool[PIPE_COUNT];

        // initialize game over title variables
        Texture2D gameOvrImg;
        Vector2 gameOvrPos;
        Vector2 gameOvrPosFin;
        Timer gameOvrFade = new Timer(GAME_OVR_FADE_TIME, false);

        // initialize game over title variables
        Texture2D rsltsBoxImg;
        Vector2 rsltBoxPos;
        Vector2 rsltBoxPosRst;
        Vector2 rsltBoxPosFin;
        Vector2 rsltCurScorPos = new Vector2(RSLT_SCOR_X, RSLT_CUR_SCOR_Y);
        Vector2 rsltHighScorPos = new Vector2(RSLT_SCOR_X, RSLT_HIGH_SCOR_Y);

        // initialize result score variables
        int rsltScor = SCOR_RST;
        Timer cntDelay = new Timer(CNT_DELAY, false);

        // initialize medals
        Texture2D coinImgs;
        Vector2 coinPos = new Vector2();
        Vector2 coinSize;
        int coinRadius;
        Vector2 coinOrigin;

        // initialize the time it takes to move the menu button up
        Timer menuUpTime = new Timer(MENU_POP_UP_TIME, false);

        // new best tag
        Texture2D newBestImg;
        Vector2 newBestPos = new Vector2(NEW_BEST_X, NEW_BEST_Y);

        // initialize all audio varibles
        Song bgMusic;
        SoundEffect fadeSnd;
        SoundEffect flapSnd;
        SoundEffect pointSnd;
        SoundEffect hitSnd;
        SoundEffect dieSnd;

        // create a hit sound timer
        Timer hitSndTimer;

        // sparkle variables
        Texture2D sparkAnimImg;
        Animation sparkAnim;
        Vector2 sparkPos;

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

            // set screen dimensions to the defined width and height
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;

            // apply the graphics change
            graphics.ApplyChanges();

            // set mouse to visible
            IsMouseVisible = true;

            // read the stat file
            ReadStatsFile(STATS_FILE_PATH);

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

            statsBoxImg = Content.Load<Texture2D>("Images/Sprites/StatsPage");
            statsBoxPos = new Vector2(SCREEN_WIDTH / 2 - statsBoxImg.Width / 2, 240);

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

            // reset the pipes into positions
            ResetPipes();

            // load game over title image
            gameOvrImg = Content.Load<Texture2D>("Images/Sprites/GameOver");
            gameOvrPosFin = new Vector2(SCREEN_WIDTH / 2 - gameOvrImg.Width / 2, SCREEN_HEIGHT / 3 - gameOvrImg.Height / 2);
            gameOvrPos = gameOvrPosFin;

            // load result box images
            rsltsBoxImg = Content.Load<Texture2D>("Images/Sprites/ResultsBox");
            rsltBoxPosRst = new Vector2(SCREEN_WIDTH / 2 - rsltsBoxImg.Width / 2, SCREEN_HEIGHT);
            rsltBoxPosFin = new Vector2(rsltBoxPosRst.X, gameOvrPos.Y + gameOvrImg.Height + GAME_OVR_RSLT_SPC_Y);
            rsltBoxPos = rsltBoxPosFin;


            // load the coin images and initialize all other coin variables
            coinImgs = Content.Load<Texture2D>("Images/Sprites/Coins");
            coinPos = new Vector2(rsltBoxPosFin.X + COIN_OFFSET_X, rsltBoxPosFin.Y + COIN_OFFSET_Y);
            coinSize = new Vector2(coinImgs.Width / COINS_COUNT, coinImgs.Height);
            coinRadius = (int)coinSize.X / 2;
            coinOrigin = new Vector2(coinPos.X + coinSize.X / 2, coinPos.Y + coinSize.Y / 2);

            // load sparkle image
            sparkAnimImg = Content.Load<Texture2D>("Images/Sprites/Sparkle");
            sparkAnim = new Animation(sparkAnimImg, 5, 1, 5, 1, Animation.NO_IDLE, Animation.ANIMATE_ONCE, SPARK_DUR, sparkPos, false);

            // new high score image
            newBestImg = Content.Load<Texture2D>("Images/Sprites/New");

            // load all audio
            bgMusic = Content.Load<Song>("Music/Nature");
            fadeSnd = Content.Load<SoundEffect>("Sound/MenuSwoosh");
            flapSnd = Content.Load<SoundEffect>("Sound/Flap");
            pointSnd = Content.Load<SoundEffect>("Sound/Point");
            hitSnd = Content.Load<SoundEffect>("Sound/Hit");
            dieSnd = Content.Load<SoundEffect>("Sound/Die");

            // set hit sound timer to the duration of the hit sound
            hitSndTimer = new Timer(hitSnd.Duration.TotalMilliseconds, false);

            //Set sound options
            MediaPlayer.Volume = 0.6f; // a float from 0 to 1 where 0 is silent
            MediaPlayer.IsRepeating = true; //Sets it to play forever
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
            {
                Exit();
            }

            // update mouse variables
            prevMouse = mouse;
            mouse = Mouse.GetState();

            // update keyboard variables
            prevKb = kb;
            kb = Keyboard.GetState();

            // check if the music is playing if not play it
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(bgMusic);
            }


            // update the current game state
            switch (gameState)
            {
                case STATS_STATE:
                    UpdateStats(gameTime);
                    break;

                case MENU_STATE:
                    // update title screen
                    UpdateMenu(gameTime);
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
                    // update pre game over
                    UpdatePreGameOvr(gameTime);
                    break;

                case GAME_OVER_STATE:
                    // update game over screen
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

            // draw the current game state
            switch (gameState)
            {
                case STATS_STATE:
                    //draw stats screen
                    DrawStats();
                    break;

                case MENU_STATE:
                    // draw title screen
                    DrawMenuScreen();
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
                    // draw the pre game over screen
                    DrawPreGameOvr();
                    break;

                case GAME_OVER_STATE:
                    // draw the game over screen
                    DrawGameOvr(gameTime);
                    break;
            }


            spriteBatch.End();
            base.Draw(gameTime);
        }


        // method to update stats state
        private void UpdateStats(GameTime gameTime)
        {
            // update ground scrolling
            UpdateGround();

            // check if there is no fade
            if (fadeCurTime.IsFinished() || fadeCurTime.IsInactive())
            {
                // check if player clicks the menu button or preses space
                if (IsClickedOn(btnRecs[MENU_BTN]) || IsKeyPressed(Keys.Space))
                {
                    // reset the fade time and activate it
                    ResetFade(FADE_STATE_TIME, true);

                    // start fade sound
                    PlaySound(fadeSnd, FADE_SND_VOL);

                    // set next game state to MENU
                    nxtGameState = MENU_STATE;
                }
            }

            // if fade timer is on update fade
            if (fadeCurTime.IsActive())
            {
                UpdateFade(gameTime, FADE_STATE_TIME, nxtGameState);
            }
        }

        // method to reset any menu elements
        private void RstMenu()
        {
            // reset the bird position to title position
            birdPos = birdTitlePosRst;

            // re activate the bird animation, and restart it aswell
            birdAnim.Activate(true);
        }

        // method to update the menu state
        private void UpdateMenu(GameTime gameTime)
        {
            // update bird animation
            birdAnim.Update(gameTime);

            // move title and bird up and down
            titlePos.Y = TITLE_WAVE_AMP * (float)(Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / TITLE_WAVE_TIME * Math.PI)) + titlePosRst.Y;
            birdPos.Y = TITLE_WAVE_AMP * (float)(Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / TITLE_WAVE_TIME * Math.PI)) + birdTitlePosRst.Y;

            birdAnim.TranslateTo(birdPos.X, birdPos.Y);

            // update ground scrolling
            UpdateGround();

            // check if there is no fade
            if (fadeCurTime.IsFinished() || fadeCurTime.IsInactive())
            {
                // check if player clicks the start button or presses space, set nextGameState to pre game play
                // if so, reset the fade respectively
                if (IsClickedOn(btnRecs[START_BTN]) || IsKeyPressed(Keys.Space))
                {
                    // reset fade and active it
                    ResetFade(FADE_STATE_TIME, true);

                    // start fade sound
                    PlaySound(fadeSnd, FADE_SND_VOL);

                    // set next game state to pre game
                    nxtGameState = PRE_GAME_PLAY_STATE;
                }
                else if (IsClickedOn(btnRecs[STATS_BTN]))
                {
                    // reset fade and active it
                    ResetFade(FADE_STATE_TIME, true);

                    // start fade sound
                    PlaySound(fadeSnd, FADE_SND_VOL);

                    // set next game state to pre game
                    nxtGameState = STATS_STATE;
                }
            }

            // if fade timer is on update it
            if (fadeCurTime.IsActive())
            {
                // update fade time
                UpdateFade(gameTime, FADE_STATE_TIME, nxtGameState);
            }
        }

        // method to reset the any pre game elements
        private void RstPreGamePlay()
        {
            // set bird to pre game position
            birdPos.X = birdPrePosRst.X;
            birdPos.Y = birdPrePosRst.Y;
            birdAnim.TranslateTo(birdPos.X, birdPos.Y);

            // reset bird speed and angle
            birdSpd = FLAP;
            birdAnim.SetAngleDeg(0);

            // reset current score variables
            ResetScore();
        }

        // method update the pre game play screen
        private void UpdatePreGame(GameTime gameTime)
        {
            // update the bird animation
            birdAnim.Update(gameTime);

            // update ground scrolling
            UpdateGround();

            // only update the fade if the timer is active
            if (fadeCurTime.IsActive())
            {
                UpdateFade(gameTime, FADE_STATE_TIME);
            }

            //  other wise check if the player has pressed space or clicked to start
            else if (fadeCurTime.IsFinished() && (IsClick() || IsKeyPressed(Keys.Space)))
            {
                // change the game state to game play
                gameState = GAME_PLAY_STATE;

                // reset the positions of pipes
                ResetPipes();

                // change the duration of the bird animation
                birdAnim.SetDuration(BIRD_ANIM_DUR_GAME);

                // reset flap count && bird death flag
                isBirdDead = false;

                // start flap sound
                PlaySound(flapSnd, FLAP_SND_VOL);
            }
        }

        // method update the current game play
        private void UpdateGamePlay(GameTime gameTime)
        {
            // update the birds animation
            birdAnim.Update(gameTime);

            // update the pipes
            UpdatePipes(gameTime);

            // update the bird position
            UpdateBirdPos(gameTime);

            // update the scrolling ground
            UpdateGround();
        }

        // update the pre game over screen
        private void UpdatePreGameOvr(GameTime gameTime)
        {
            // update the death fade (flash)
            UpdateFadeDeath(gameTime);

            // update the hit sound timer
            hitSndTimer.Update(gameTime);

            // check how the bird died
            switch (birdDeathType)
            {
                case GRD_DEATH:
                    // update the death timer directly
                    birdDeathTimer.Update(gameTime);
                    break;
                case PIPE_DEATH:
                    // still need to wait for bird to hit ground
                    // thus update the bird position
                    UpdateBirdPos(gameTime);
                    break;
            }

            // check if the hit sound timer is finished
            if (hitSndTimer.IsFinished())
            {
                // play death sound
                PlaySound(dieSnd, DIE_SND_VOL);

                // reset the hit sound timer
                hitSndTimer.ResetTimer(false);
            }


            // change to game over state after death timer finish
            if (birdDeathTimer.IsFinished())
            {
                // update game sate
                gameState = GAME_OVER_STATE;

                // set newBest flag to true, if new high score is reached
                if (bestScor < curScor)
                {
                    isNewBest = true;
                }

                // reset game over state
                ResetGameOvr();
            }
        }

        // method to reset any game over elements
        private void ResetGameOvr()
        {
            // reset the count delay timer. However only activate it if the current score is non-zero
            if (curScor != 0)
            {
                cntDelay.ResetTimer(true);
            }
            else
            {
                cntDelay.ResetTimer(false);
            }

            // reset the game over fade title timer
            gameOvrFade.ResetTimer(true);

            // reset the result box position
            rsltBoxPos = rsltBoxPosRst;

            // reset the menu button timer and position
            menuUpTime.ResetTimer(false);
            btnRecs[MENU_BTN].Y = MENU_BTN_Y_RST;
        }

        // method to update game over state
        private void UpdateGameOvr(GameTime gameTime)
        {
            // check if the hit sound timer is still active, if so update it
            if (hitSndTimer.IsActive())
            {
                hitSndTimer.Update(gameTime);
            }
            // other wise play death sound
            else if (hitSndTimer.IsFinished())
            {
                // play death sound and reset hit sound timer
                PlaySound(dieSnd, DIE_SND_VOL);
                hitSndTimer.ResetTimer(false);
            }

            // update the game over title
            UpdateGameOvrTitle(gameTime);

            // update the result box and it's elements
            UpdateRsltBox(gameTime);

            // update score elements in the result box
            UpdateRsltScor(gameTime);

            // update the menu button pop up timer
            menuUpTime.Update(gameTime);

            // activate the menu timer is the score counter if finished
            if (rsltBoxPos.Y == rsltBoxPosFin.Y && rsltScor == curScor && menuUpTime.IsInactive())
            {
                menuUpTime.Activate();
            }

            // check if menu timer is active
            if (menuUpTime.IsActive())
            {
                // update the y position of the button passed on the completion of the timer
                btnRecs[MENU_BTN].Y = (int)MathHelper.Lerp(MENU_BTN_Y_RST, MENU_BTN_Y_FIN, (float)menuUpTime.GetTimePassed() / MENU_POP_UP_TIME);
            }
            // check if player has press menu button or pressed space, if the score counting is finished and result box is in place
            else if ((IsClickedOn(btnRecs[MENU_BTN]) || IsKeyPressed(Keys.Space)) && menuUpTime.IsFinished() && rsltBoxPos.Y == rsltBoxPosFin.Y)
            {
                // add the score the score list
                AddScore(curScor);

                // score the list
                SortListDecending();

                // increment play count
                playsCnt++;

                // calculate the average variables
                avgFlaps = flapCnt / playsCnt;
                avgScor = SumScor() / scorsList.Count();

                // write the stats to file
                WriteStatsFile(STATS_FILE_PATH);

                // reset the fade, and activate it
                ResetFade(FADE_STATE_TIME, true);

                // start fade sound
                PlaySound(fadeSnd, FADE_SND_VOL);
            }
            // else check if fade between states is active, and update fade is so
            else if (fadeCurTime.IsActive())
            {
                UpdateFade(gameTime, FADE_STATE_TIME, MENU_STATE);
            }
        }

        // method draw the stats screen
        private void DrawStats()
        {
            // draw the ground
            DrawGround();

            // draw the menu button
            spriteBatch.Draw(btnImgs[MENU_BTN], btnRecs[MENU_BTN], Color.White);

            // draw the stats box
            DrawStatsBox();

            // draw fade
            DrawFadeScr(Color.Black);
        }

        // method to draw the title screen
        private void DrawMenuScreen()
        {
            // draw the ground
            DrawGround();

            // draw the title
            spriteBatch.Draw(titleImg, titlePos, Color.White);

            // draw animated bird
            birdAnim.Draw(spriteBatch, Color.White);

            // draw the starting and stats button in the title screen
            spriteBatch.Draw(btnImgs[START_BTN], btnRecs[START_BTN], Color.White);
            spriteBatch.Draw(btnImgs[STATS_BTN], btnRecs[STATS_BTN], Color.White);

            // only draw the fade is the timer is active
            if (fadeCurTime.IsActive())
            {
                DrawFadeScr(Color.Black);
            }
        }

        // method to draw the pre game play screen
        private void DrawPreGamePlay(GameTime gameTime)
        {
            // draw the bird
            birdAnim.Draw(spriteBatch, Color.White);

            // draw the ground
            DrawGround();

            // draw click instructions
            spriteBatch.Draw(intrucImg, intrcuPos, Color.White);

            // draw get ready title
            spriteBatch.Draw(readyTitleImg, readyTitlePos, Color.White);

            // check if the timer is still running, if so draw the fade
            if (fadeCurTime.IsActive())
            {
                DrawFadeScr(Color.Black);
            }
            // otherwise reset the fade variables

            // draw the current score, such that the unit digits is centered
            DrawNumberSequence(curScor.ToString(), numsLrgImg, curScorPos, NUM_SPC_FACTOR, 0.5f);
        }

        // method to draw the game play
        private void DrawGamePlay(GameTime gameTime)
        {
            // draw the pipes 
            DrawPipes();

            // draw the pipes
            DrawGround();

            // draw the bird with rotation
            birdAnim.DrawRotated(spriteBatch, Color.White);

            // draw the current score
            DrawNumberSequence(curScor.ToString(), numsLrgImg, curScorPos, NUM_SPC_FACTOR, 0.5f);
        }

        // method to draw the pre game over screen
        private void DrawPreGameOvr()
        {
            // draw the pipes
            DrawPipes();

            // draw the ground
            DrawGround();

            // draw the bird rotated
            birdAnim.DrawRotated(spriteBatch, Color.White);

            // draw the death fade (flash)
            DrawFadeScr(Color.White);
        }

        // method to draw the game over screen
        private void DrawGameOvr(GameTime gameTime)
        {
            // draw the pipes
            DrawPipes();

            // draw the ground
            DrawGround();

            // draw bird rotated
            birdAnim.DrawRotated(spriteBatch, Color.White);

            // draw the game over title
            DrawGameOvrTitle();

            // draw the result box and it's elements
            DrawRsltBox();

            // result box is in place draw the menu
            if (rsltBoxPos == rsltBoxPosFin)
            {
                spriteBatch.Draw(btnImgs[MENU_BTN], btnRecs[MENU_BTN], Color.White);
            }

            // check if the timer is still running, if so draw the fade
            if (fadeCurTime.IsActive())
            {
                DrawFadeScr(Color.Black);
            }
        }

        // method to update the position of the bird
        private void UpdateBirdPos(GameTime gameTime)
        {
            // check weather if the bird has not died
            if (!isBirdDead)
            {
                // check if the player clicks or presses space
                if (IsClick() || IsKeyPressed(Keys.Space))
                {
                    // set bird speed to flap speed
                    birdSpd = FLAP;

                    // increase flap count
                    flapCnt++;

                    // start flap sound
                    PlaySound(flapSnd, FLAP_SND_VOL);
                }
            }

            // decrease the speed by gravity and restrict it between a defined min and max value
            birdSpd += GRAVITY;
            birdSpd = MathHelper.Clamp(birdSpd, MIN_SPD, MAX_SPD);

            // change the position of the bird with the speed
            birdPos.Y += birdSpd;

            // translate the bird to that point
            birdAnim.TranslateTo(birdPos.X, birdPos.Y);

            // update the bird's tilt angle
            UpdateBirdDeg(MIN_ANGL, MAX_ANGL, birdSpd);

            // update any bird collision
            BirdCollision(gameTime);
        }

        // method to update the bird's tilt angle
        private void UpdateBirdDeg(int minAngle, int maxAngle, float spd)
        {
            birdAnim.SetAngleDeg(MathHelper.Lerp(minAngle, maxAngle, (spd - MIN_SPD) / (MAX_SPD - MIN_SPD)));
        }

        private void BirdCollision(GameTime gameTime)
        {
            // check if bird has died, if it has not need to check if it hits the ceiling
            if (!isBirdDead && birdAnim.GetDestRec().Top <= 0)
            {
                // store the death type
                birdDeathType = PIPE_DEATH;

                DeathDelayReset(gameTime, birdDeathType);

                PlaySound(hitSnd);
                hitSndTimer.Activate();
            }
            // iterate through the pipes check if bird has died, if it has not need to check if it hits the ceiling
            for (int i = 0; i < PIPE_COUNT && !isBirdDead; i++)
            {
                if (pipeRecs[i, TOP].Intersects(birdAnim.GetDestRec()) || pipeRecs[i, BOT].Intersects(birdAnim.GetDestRec()))
                {
                    // store the death type
                    birdDeathType = PIPE_DEATH;

                    PlaySound(hitSnd);
                    hitSndTimer.Activate();

                    DeathDelayReset(gameTime, birdDeathType);
                    break;
                }
            }

            // check if collision with ground
            if (birdAnim.GetDestRec().Bottom >= GRD_Y)
            {
                birdAnim.TranslateTo(birdPos.X, GRD_Y - birdAnim.GetDestRec().Height);

                // store the death type
                birdDeathType = GRD_DEATH;

                // change the game state
                gameState = PRE_GAME_OVER_STATE;

                // only reset death and play hit sound if bird didn't die yet
                // it could be possible if bird died from pipe before it hit ground
                if (!isBirdDead)
                {
                    DeathDelayReset(gameTime, birdDeathType);

                    PlaySound(hitSnd);
                    hitSndTimer.Activate();
                }
                else
                {
                    birdDeathTimer.Activate();
                }

                if (hitSndTimer.IsFinished())
                {
                    PlaySound(dieSnd);
                    hitSndTimer.ResetTimer(false);
                }
            }
        }

        private void DeathDelayReset(GameTime gameTime, int deathType)
        {
            // deactivate the bird's animation and set idle frame to 1
            birdAnim.Deactivate();
            birdAnim.SetIdleFrame(1); // NOTE NEED TO FIRST DEACTIVE then set idle frame. 


            birdDeathTimer.ResetTimer(false);

            ResetFade(FADE_DEATH_TIME, true);

            gameState = PRE_GAME_OVER_STATE;

            isBirdDead = true;

            if (deathType == GRD_DEATH)
            {
                birdDeathTimer.Activate();
            }
        }

        private void UpdateFadeDeath(GameTime gameTime)
        {
            UpdateFade(gameTime, FADE_DEATH_TIME);
        }

        // updates the fade time, and returns true if fade timer is completed
        // if isIn is true, then it's fading in, otherwise it's fading out
        private void UpdateFade(GameTime gameTime, int fadeTime, int nextGameState = -1)
        {
            fadeCurTime.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Console.WriteLine(fadeCurTime.GetTimePassed().ToString() + " " + gameState + " " + fadeOpacity);

            if (fadeCurTime.GetTimePassed() < fadeTime / 2)
            {
                // fade out
                // store the opacity of the fade, as it should be
                fadeOpacity = Math.Min(1f, (float)fadeCurTime.GetTimePassed() / (fadeTime / 2) * FULL_OPACITY);
            }
            else
            {
                // fade in
                if (nextGameState != -1 && !fadedHalf)
                {
                    gameState = nextGameState;
                    switch (gameState)
                    {
                        case PRE_GAME_PLAY_STATE:
                            RstPreGamePlay();
                            break;
                        case GAME_OVER_STATE:
                            break;
                        case MENU_STATE:
                            RstMenu();
                            break;
                    }
                }

                fadeOpacity = Math.Min(1f, (fadeTime - (float)fadeCurTime.GetTimePassed()) / (fadeTime / 2)) * FULL_OPACITY;
                fadedHalf = true;
            }
        }

        // draw the fade screen
        // if isIn is true, then it's fading in, otherwise it's fading out
        private void DrawFadeScr(Color color)
        {
            spriteBatch.Draw(fadeImg, scrnRec, color * fadeOpacity);
        }

        // reset all fade variables
        private void ResetFade(int fadeTime, bool isActive)
        {
            // reset the fade opacity and fade timer
            fadeOpacity = EMPTY_OPACITY;
            fadeCurTime.SetTargetTime(fadeTime);
            fadeCurTime.ResetTimer(isActive);

            // reset the fade half flag
            fadedHalf = false;
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

        // method to draw the ground
        private void DrawGround()
        {
            // draw the ground
            spriteBatch.Draw(grdImg, grdRecs[0], Color.White);
            spriteBatch.Draw(grdImg, grdRecs[1], Color.White);
        }

        // method to update pipes
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
                // check if player can't passed the pipe and the front of the bird passes the right side of pipe
                else if (!pipesPass[i] && pipeRecs[i, TOP].Right <= birdAnim.GetDestRec().Right)
                {
                    // increment the score and set pipe to passed
                    UpdateScore(10);
                    pipesPass[i] = true;

                    // play point sound
                    PlaySound(pointSnd, POINT_SND_VOL);
                }

                // set the x value of pipes (top and bottom) rectangles to their positions
                pipeRecs[i, TOP].X = (int)pipePos[i, TOP].X;
                pipeRecs[i, BOT].X = (int)pipePos[i, BOT].X;
            }
        }

        // method to generate pipe posistions when given pipe index
        private void GeneratePipe(int pipeIndex)
        {
            // reset pip passed
            pipesPass[pipeIndex] = false;

            // the right most pipe would be one index smaller
            int rightMost = pipeIndex - 1;
            if (pipeIndex == 0)
            {
                rightMost = PIPE_COUNT - 1;
            }

            // reposition image back to the other end of the other image
            pipePos[pipeIndex, TOP].X = pipePos[rightMost, TOP].X + pipeRecs[rightMost, TOP].Width + PIPE_X_SPC;
            pipePos[pipeIndex, BOT].X = pipePos[rightMost, BOT].X + pipeRecs[rightMost, BOT].Width + PIPE_X_SPC;

            // vertical displacement
            int verDis = rand.Next(-400, 400);

            // clamp the y value of top pipe between max and min values, and set bottom to be below the top by given amount
            pipePos[pipeIndex, TOP].Y = MathHelper.Clamp(pipePos[rightMost, TOP].Y + verDis, -1 * pipeRecs[rightMost, TOP].Height + PIPE_MIN_Y_FACTR, PIPE_MAX_Y);
            pipePos[pipeIndex, BOT].Y = pipePos[pipeIndex, TOP].Y + pipeRecs[pipeIndex, TOP].Height + PIPE_Y_SPC;

            // translate the rectangle to their respective positions
            pipeRecs[pipeIndex, TOP].X = (int)pipePos[pipeIndex, TOP].X;
            pipeRecs[pipeIndex, TOP].Y = (int)pipePos[pipeIndex, TOP].Y;
            pipeRecs[pipeIndex, BOT].X = (int)pipePos[pipeIndex, BOT].X;
            pipeRecs[pipeIndex, BOT].Y = (int)pipePos[pipeIndex, BOT].Y;
        }

        // method to reset the pipes
        private void ResetPipes()
        {
            // iterate through the pipes
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

                    // set x values of the pipes to the right of the previous pipe
                    pipePos[i, TOP].X = pipePos[i - 1, TOP].X + pipeRecs[i - 1, TOP].Width + PIPE_X_SPC;
                    pipePos[i, BOT].X = pipePos[i - 1, BOT].X + pipeRecs[i - 1, BOT].Width + PIPE_X_SPC;
                }
                else // else i is at the 0 index 
                {
                    // the right most will be the always be between 0 and - the negative height of pipe plus '88'
                    pipePos[i, TOP].Y = rand.Next(-1 * pipeRecs[0, TOP].Height + PIPE_MIN_Y_FACTR, PIPE_MAX_Y);
                    pipePos[i, BOT].Y = pipePos[i, TOP].Y + pipeRecs[i, TOP].Height + PIPE_Y_SPC;

                    // set the x value to the reset position
                    pipePos[i, TOP].X = PIPE_RST_X;
                    pipePos[i, BOT].X = PIPE_RST_X;
                }

                // change all the rectangle position to their respective position values
                pipeRecs[i, TOP].X = (int)pipePos[i, TOP].X;
                pipeRecs[i, BOT].X = (int)pipePos[i, BOT].X;
                pipeRecs[i, TOP].Y = (int)pipePos[i, TOP].Y;
                pipeRecs[i, BOT].Y = (int)pipePos[i, BOT].Y;
            }
        }

        // method to draw the pipes
        private void DrawPipes()
        {
            // iterate through the pipes
            for (int i = 0; i < PIPE_COUNT; i++)
            {
                // draw the bottom and top image of the ith pipe
                spriteBatch.Draw(pipeImgs[TOP], pipeRecs[i, TOP], Color.White);
                spriteBatch.Draw(pipeImgs[BOT], pipeRecs[i, BOT], Color.White);
            }
        }

        // method to draw stats rectangle box
        private void DrawStatsBox()
        {
            // draw the stats image
            spriteBatch.Draw(statsBoxImg, statsBoxPos, Color.White);

            // draw the total play count and total flap count with small numbers
            DrawNumberSequence(playsCnt.ToString(), numsSmlImg, playsPos, NUM_SPC_FACTOR, 0);
            DrawNumberSequence(flapCnt.ToString(), numsSmlImg, flapsPos, NUM_SPC_FACTOR, 0);

            // draw the average score, and average flaps with small numbers
            DrawNumberSequence(avgScor.ToString(), numsSmlImg, avgScorsPos, NUM_SPC_FACTOR, 0);
            DrawNumberSequence(avgFlaps.ToString(), numsSmlImg, avgFlapsPos, NUM_SPC_FACTOR, 0);

            // iterate until the end of the score list or until the top 5 scores (TOP_SCORS)
            for (int i = 0; i < scorsList.Count && i < TOP_SCORS; i++)
            {
                // draw the ith score
                DrawNumberSequence(scorsList[i].ToString(), numsSmlImg, new Vector2(topScorsPos.X, topScorsPos.Y + i * TOP_SCORS_Y_SPC), NUM_SPC_FACTOR, 0);
            }
        }

        // method reset the score variables
        private void ResetScore()
        {
            // reset the position of the score
            curScorPos.X = SCREEN_WIDTH / 2; // able to use 20 since (x/10/2) is (x/20)

            // reset the current and result scores
            curScor = SCOR_RST;
            rsltScor = SCOR_RST;

            // reset best score flag to false
            isNewBest = false;
        }

        // method to update score, (increment used for debug)
        private void UpdateScore(int increment = 1)
        {
            // add increment to the current score value
            curScor += increment;
        }

        // method to draw numbers with the num texture when given a number in the form a string
        private void DrawNumberSequence(string num, Texture2D texture, Vector2 pos, int spacingFactor, float rightOffset = 0)
        {
            // store the width of a digit
            int digitWidth = texture.Width / 10; // 10 digits in sprite

            // space between each digit, as a factor of the digit width
            int numSpacer = digitWidth / spacingFactor;

            // change the position of the number by the numbers length, and offset
            pos.X -= num.Length * (texture.Width / 10 + numSpacer) - numSpacer;
            pos.X += digitWidth * rightOffset;

            // iterate through the number 
            for (int i = 0; i < num.Length; i++)
            {
                // store the digits value, by changing char to int
                int digit = num[i] - '0';

                // store the digit's draw position and score rectangle based of which number place it is
                Vector2 drawLoc = new Vector2(pos.X + i * (digitWidth + numSpacer), pos.Y);
                Rectangle sourceRec = new Rectangle(digit * digitWidth, 0, digitWidth, texture.Height);

                // draw the digit
                spriteBatch.Draw(texture, drawLoc, sourceRec, Color.White);
            }
        }

        // method to update game over title
        private void UpdateGameOvrTitle(GameTime gameTime)
        {
            // update the game over title fade
            gameOvrFade.Update(gameTime);

            // update game over title position
            gameOvrPos.Y = GAME_OVR_WAVE_AMP * (float)Math.Sin((gameOvrFade.GetTimePassed() + GAME_OVR_FADE_TIME) * (4.0 / (3.0 * GAME_OVR_FADE_TIME)) * Math.PI) + GAME_OVR_WAVE_DISPL;
            // more info of the sine wave
            // https://www.desmos.com/calculator/lk5euhols6
        }

        // method to update result box and it's elements
        private void UpdateRsltBox(GameTime gameTime)
        {
            // check if game over title fade is finished and result box hasn't reached 
            if (gameOvrFade.IsFinished() && rsltBoxPos != rsltBoxPosFin)
            {
                // update the position of the box with the speed
                rsltBoxPos.Y += RSLT_BOX_SPD;

                // if the box crossed, set it equal to the final position
                if (rsltBoxPos.Y < rsltBoxPosFin.Y)
                {
                    rsltBoxPos.Y = rsltBoxPosFin.Y;
                }
            }
            // update sparkle if the result score counted up to current score, and a medal is earned
            else if (rsltScor == curScor && curScor >= BRONZE_SCR)
            {
                UpdateSparkle(gameTime);
            }
        }

        // method to update the result scores
        private void UpdateRsltScor(GameTime gameTime)
        {
            // check if result box is in place
            if (rsltBoxPos.Y == rsltBoxPosFin.Y)
            {
                // check if the result score is lower, and count delay is finished
                // then the results score counts up to current score
                if (rsltScor < curScor && cntDelay.IsFinished())
                {
                    // increase the count of the result score
                    rsltScor++;

                    // check if the result score surpassed the high score, if so update the high score
                    if (isNewBest && rsltScor > bestScor)
                    {
                        bestScor++;
                    }

                    // reset the timer
                    cntDelay.ResetTimer(true);

                }
                // update the count delay between increments
                else
                {
                    cntDelay.Update(gameTime);
                }
            }
        }

        // method to draw the game title
        private void DrawGameOvrTitle()
        {
            spriteBatch.Draw(gameOvrImg, gameOvrPos, Color.White * (float)(gameOvrFade.GetTimePassed() / GAME_OVR_FADE_TIME));
        }


        // method draw the result box and it's elements
        private void DrawRsltBox()
        {
            spriteBatch.Draw(rsltsBoxImg, rsltBoxPos, Color.White);

            // check if the result box is at it's final position, meaning that it finished moving
            if (rsltBoxPos == rsltBoxPosFin)
            {
                // draw the result and best score with large numbers
                // spacing between the digits of a 1/4 of the digit size
                // offset is zero as digits are right of the points
                DrawNumberSequence(rsltScor.ToString(), numsLrgImg, rsltCurScorPos, NUM_SPC_FACTOR, 0);
                DrawNumberSequence(bestScor.ToString(), numsLrgImg, rsltHighScorPos, NUM_SPC_FACTOR, 0);

                // only draw the coins if the score counter if finished
                if (rsltScor == curScor)
                {
                    // check if score is greater or equal to each coin score
                    // if so draw that coin
                    if (curScor >= PLAT_SCR)
                    {
                        DrawCoin(PLAT_COIN);
                    }
                    else if (curScor >= GOLD_SCR)
                    {
                        DrawCoin(GOLD_COIN);
                    }
                    else if (curScor >= SILVER_SCR)
                    {
                        DrawCoin(SILVER_COIN);
                    }
                    else if (curScor >= BRONZE_SCR)
                    {
                        DrawCoin(BRONZE_COIN);
                    }
                }

                // draw the nest best if a nest best is reached and result score has reached the best score
                if (isNewBest && bestScor == rsltScor)
                {
                    DrawNewPb();
                }
            }
        }

        // method to update the sparkles
        private void UpdateSparkle(GameTime gameTime)
        {
            // check if animation is currently not animating
            if (!sparkAnim.IsAnimating())
            {
                // reset the position of the animation
                sparkPos.Y = rand.Next((int)coinPos.Y, (int)(coinPos.Y + coinSize.Y));
                sparkPos.X = rand.Next((int)coinPos.X, (int)(coinPos.X + coinSize.X));

                // check if the distance between the coin's origin 
                while (PointDistance(sparkPos, coinOrigin) > coinRadius)
                {
                    // re generate a coin position
                    sparkPos.Y = rand.Next((int)coinPos.Y, (int)(coinPos.Y + coinSize.Y));
                    sparkPos.X = rand.Next((int)coinPos.X, (int)(coinPos.X + coinSize.X));
                }

                // translate the animation's center to spark pos
                sparkAnim.TranslateTo(sparkPos.X - sparkAnim.GetDestRec().Width / 2, sparkPos.Y - sparkAnim.GetDestRec().Height / 2);

                // active the spark animation and reset it
                sparkAnim.Activate(true);
            }
            // if animating (not not animating), update the animation 
            else
            {
                sparkAnim.Update(gameTime);
            }
        }

        // method to draw the spark animation
        private void DrawSparkle()
        {
            sparkAnim.Draw(spriteBatch, Color.White);
        }

        // method to draw the coin's, when given coin type
        private void DrawCoin(int coinType)
        {
            // use a source rectangle to find crop the coins images
            // as the coin type is by index within the coin's image, by multiplying it by the coin size the x value of the source rectangle is given
            Rectangle sourceRec = new Rectangle(coinType * (int)coinSize.X, 0, (int)coinSize.X, (int)coinSize.Y);

            // draw the actual coin
            spriteBatch.Draw(coinImgs, coinPos, sourceRec, Color.White);

            // call the draw spark's method to draw sparks
            DrawSparkle();
        }

        // method to draw the new best tag
        private void DrawNewPb()
        {
            // draw the new Best tag
            spriteBatch.Draw(newBestImg, newBestPos, Color.White);
        }

        // method add source to the local score list 
        private void AddScore(int newScor)
        {
            // add the value to scorsList
            scorsList.Add(newScor);
        }

        // method that returns the sum of all scores within the scores list
        private int SumScor()
        {
            // create a local temporary sum variable
            int sum = 0;

            // sum the values of scorsList
            foreach (int scor in scorsList)
            {
                sum += scor;
            }

            // return the value of sum
            return sum;
        }

        // method to sort the score list from greats to least
        private void SortListDecending()
        {
            // sort the score list (increasing)
            scorsList.Sort();

            // reverse the list to have it decreasing
            scorsList.Reverse();
        }

        // method to write data to a stat txt file, when given file path
        private void WriteStatsFile(string filePath)
        {
            // create new text file (or over ride the old file)
            outFile = File.CreateText(filePath);

            // first line write the total amount of plays and flap count
            outFile.WriteLine(playsCnt + "," + flapCnt);

            // next lines write all the scores, using the scores list
            for (int i = 0; i < scorsList.Count(); i++)
            {
                outFile.WriteLine(scorsList[i]);
            }

            // close the outFile
            outFile.Close();
        }

        // method read data from a the stats file, given the file path
        private void ReadStatsFile(string filePath)
        {
            // use try-catch to handle any errors
            try
            {
                // open the stats file
                inFile = File.OpenText(filePath);

                //create a temp score and first data variable to store the stat data from the file
                int tempScor;
                string[] firstData; // use string as can't read array directly to int

                // read the first line and split it into an array
                firstData = inFile.ReadLine().Split(',');

                // try to covert the values of the first data array into ints and store them into their varibles
                // if can't convert to int, set the values to 0
                if (!int.TryParse(firstData[0], out playsCnt) || playsCnt < 0)
                {
                    playsCnt = 0;
                }

                if (!int.TryParse(firstData[1], out flapCnt) || flapCnt < 0)
                {
                    flapCnt = 0;
                }

                // read  until the end of the file
                while (!inFile.EndOfStream)
                {
                    // if value is an int and greater or equal to 0, add the value to scores list
                    if (int.TryParse(inFile.ReadLine(), out tempScor) && tempScor >= 0)
                    {
                        scorsList.Add(tempScor);
                    }
                }

                // close the file
                inFile.Close();
            }
            // catch if the stats file does not exist
            catch (FileNotFoundException fnfe)
            {
                // create the file instead
                CreateFile(filePath);

                // read the file
                ReadStatsFile(filePath);
            }
        }

        // create a txt file with the file path
        private void CreateFile(string filePath)
        {
            // create a empty txt file
            outFile = File.CreateText(filePath);

            // write the default stat values 
            outFile.WriteLine(FILE_TEMPLATE);

            // close the file
            outFile.Close();
        }

        // method which returns the distance between given two points
        private double PointDistance(Vector2 point1, Vector2 point2)
        {
            // use Pythagorean theorem to find distance between two points
            return Math.Sqrt(Math.Pow(point1.Y - point2.Y, 2) + Math.Pow(point1.X - point2.X, 2));
        }


        // method play sound effect with a specific volume
        private void PlaySound(SoundEffect snd, float volume = 1f)
        {
            SoundEffectInstance sndInstance = snd.CreateInstance();
            sndInstance.Volume = volume;
            sndInstance.Play();
        }

        // method that returns if a rectangle is click on
        private bool IsClickedOn(Rectangle rec)
        {
            // if mouse in rectangle and is clicked return true
            return rec.Contains(mouse.Position) && IsClick();
        }

        // method that returns is mouse is clicked
        private bool IsClick()
        {
            return mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed;
        }

        // method that returns if a given key is pressed
        private bool IsKeyPressed(Keys key)
        {
            return kb.IsKeyDown(key) && !prevKb.IsKeyDown(key);
        }
    }
}
