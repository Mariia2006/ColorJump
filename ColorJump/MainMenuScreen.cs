using System;
using System.Drawing;
using System.Windows.Forms;

namespace ColorJump
{
    public class ScreenMainMenu : BaseScreen
    {
        private Bitmap logo, diamond; // логотип гри та іконка діаманта
        private int selected = 0, totalDiamonds = 0; //індекс вибраної кнопки та кількість діамантів (завантажується з файлу diamonds.dat)
        private int xtut, ytut, xpl, ypl, xex, yex, logox, logoy, dimx, dimy, xshop; // координати кнопок та елементів UI
        private bool isAlreadyTapping; // запобігає багаторазовому натисканню
        private BaseSpriteButton playButton; // кнопки play, options, shop, exit
        private BaseSpriteButton optionButton, shopButton, exitButton;

        private System.Windows.Forms.Timer gameTimer; // таймер ігрового циклу (кожні 16 мс перемальовує екран)

        public ScreenMainMenu(bool isMute) : base(true, false, isMute) 
        {
            isAlreadyTapping = false;

            logo = new Bitmap("?.png"); // завантажуємо зображення
            diamond = new Bitmap("?.png");

            playButton = new BaseSpriteButton(350, 300, "?.png", 3, false); // створюємо кнопки на підвантажуємо зображення
            optionButton = new BaseSpriteButton(50, 500, "?.png", 3, false);
            shopButton = new BaseSpriteButton(350, 500, "?.png", 3, false);
            exitButton = new BaseSpriteButton(650, 500, "?.png", 3, false);

            playButton.IsPressed = true;

            logox = (ClientSize.Width - logo.Width) / 2;
            logoy = (ClientSize.Height - logo.Height) / 9;

            dimx = (ClientSize.Width - diamond.Width) / 2;
            dimy = ClientSize.Height / 12;

            xpl = (ClientSize.Width - playButton.Width) / 2;
            ypl = 5 * (ClientSize.Height - playButton.Height) / 9;
            playButton.SetPosition(xpl, ypl);

            ytut = ClientSize.Height - 6 * shopButton.Height / 5;
            xtut = shopButton.Width / 6;
            optionButton.SetPosition(xtut, ytut);

            xshop = (ClientSize.Width - shopButton.Width) / 2;
            shopButton.SetPosition(xshop, ytut);

            xex = ClientSize.Width - 7 * optionButton.Width / 6;
            yex = ClientSize.Height - 6 * optionButton.Height / 5;
            exitButton.SetPosition(xex, yex);

            totalDiamonds = ReadRecord("diamonds.dat");
            
            // тут ми запускаємо ігровий цикл
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            KeyDown += ScreenMainMenu_KeyDown;
            MouseDown += ScreenMainMenu_MouseDown;
            MouseUp += ScreenMainMenu_MouseUp;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            Update(mFrameDelay); // оновлює логіку
            Invalidate(); // перемальовує екран
        }

        public override void Update(long tick)
        {
            base.Update(tick);
        }

        protected override void OnPaint(PaintEventArgs e) // малюємо логотип і кнопки
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.DrawImage(logo, logox, logoy);

            playButton.DrawButton(g);
            optionButton.DrawButton(g);
            shopButton.DrawButton(g);
            exitButton.DrawButton(g);

            // Відображаємо кількість діамантів і малюємо іконку діаманта праворуч від числа
            using (Font font = new Font("Arial", 16, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.White))
            {
                string str = totalDiamonds.ToString();
                SizeF textSize = g.MeasureString(str, font);
                g.DrawString(str, font, brush, ClientSize.Width / 2, dimy, new StringFormat { Alignment = StringAlignment.Center });
                g.DrawImage(diamond, dimx + (int)textSize.Width + 3, dimy - (int)textSize.Height / 2);
            }
        }

        private void Fire() // запуск дії кнопки
        {
            if (!isAlreadyTapping)
            {
                isAlreadyTapping = true; // заблоковуємо повторні кліки
                gameTimer.Stop(); // зупиняємо цикл гри
                switch (selected) // визначаємо, яку сцену відкрити
                {
                    case 0: // запуск
                        /*new CanvasColorJampGame(muted).Show(); 
                        this.Hide();*/
                        break;
                    case 1: // налаштування
                        /*new CanvasSetting(muted).Show();
                        this.Hide();*/
                        break;
                    case 2: // магазин
                        /*new CanvasShop(muted, 0).Show();
                        this.Hide();*/
                        break;
                    case 3: // вихід з гри
                        Application.Exit();
                        break;
                }
            }
        }

        private void ScreenMainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) // отримує клавішу, яку ми натиснули
            {
                case Keys.Enter: // активує Fire()
                    Fire();
                    break;
                case Keys.S: // вибирає shopButton
                    if (selected == 0)
                    {
                        selected = 2;
                        shopButton.IsPressed = true;
                        playButton.IsPressed = false;
                    }
                    break;
                case Keys.W: // вибирає playButton
                    if (selected != 0)
                    {
                        selected = 0;
                        playButton.IsPressed = true;
                        optionButton.IsPressed = false;
                        shopButton.IsPressed = false;
                        exitButton.IsPressed = false;
                    }
                    break;
                case Keys.D: // перемикання між кнопками 
                    if (selected == 2)
                    {
                        selected = 3;
                        shopButton.IsPressed = false;
                        exitButton.IsPressed = true;
                    }
                    else if (selected == 1)
                    {
                        selected = 2;
                        shopButton.IsPressed = true;
                        optionButton.IsPressed = false;
                    }
                    break;
                case Keys.A:
                    if (selected == 2)
                    {
                        selected = 1;
                        optionButton.IsPressed = true;
                        shopButton.IsPressed = false;
                    }
                    else if (selected == 3)
                    {
                        selected = 2;
                        shopButton.IsPressed = true;
                        exitButton.IsPressed = false;
                    }
                    break;
            }
        }

        private void ScreenMainMenu_MouseDown(object sender, MouseEventArgs e)
        {
            if (playButton.IsTapOnButton(e.X, e.Y)) // чи координати кліку попали в кнопку
            {
                selected = 0;
                playButton.IsPressed = true;
            }
            else if (optionButton.IsTapOnButton(e.X, e.Y))
            {
                selected = 1;
                optionButton.IsPressed = true;
            }
            else if (shopButton.IsTapOnButton(e.X, e.Y))
            {
                selected = 2;
                shopButton.IsPressed = true;
            }
            else if (exitButton.IsTapOnButton(e.X, e.Y))
            {
                selected = 3;
                exitButton.IsPressed = true;
            }
        }

        private void ScreenMainMenu_MouseUp(object sender, MouseEventArgs e) // перевіряє чи була натиснута кнопка
        {
            if (playButton.IsPressed) { playButton.IsPressed = false; Fire(); }
            if (optionButton.IsPressed) { optionButton.IsPressed = false; Fire(); }
            if (shopButton.IsPressed) { shopButton.IsPressed = false; Fire(); }
            if (exitButton.IsPressed) { exitButton.IsPressed = false; Fire(); }
        }
    }
}