using System;
using System.Drawing;
using System.IO; // збереження/завантаження даних
using System.Media;
using System.Threading; // для запуску ігрового циклу в окремому потоці
using System.Windows.Forms;
using System.Linq;

public abstract class BaseCanvas : Form // успадкування від форми 
{
    protected const int mFrameDelay = 16; // 60 FPS
    protected long tick; // лічильник кадрів
    protected bool muted = false; // флаг вимкнення музики
    protected bool paused = false; // флаг паузи
    protected static SoundPlayer Play_Music_Main; // програвач музики
    private int key = 0; // збереження останньої натиснутої клавіши
    private const int SleepTime = 40;
    private bool _isWorking; // флаг запущеного ігрового циклу

    protected BaseCanvas(bool suppressKeyEvents, bool gameMusic, bool isMute)
    {
        muted = isMute;
        DoubleBuffered = true; // уникаємо мерехтіння кадрів
        ClientSize = new Size(800, 600); // фіксований розмір вікна
        FormBorderStyle = FormBorderStyle.FixedSingle; // забороняє зміну розміру
        MaximizeBox = false; // відключає кнопку розгорнути
        MinimizeBox = true; // дозволяє згорнути
        KeyPreview = true; // обробка клавіш для вкладених елементів
        KeyDown += BaseCanvas_KeyDown; // обробник натискання клавіш
        KeyUp += BaseCanvas_KeyUp; // обробник відпускання клавіш

        if (gameMusic)
        {
            InitPlayMusicMain();
        }
    }

    private void BaseCanvas_KeyDown(object sender, KeyEventArgs e)
    {
        key = e.KeyValue; // зберігаємо код натиснутої клавіші
        Input(key); // викликаємо метод обробки клавіш
    }

    private void BaseCanvas_KeyUp(object sender, KeyEventArgs e)
    {
        key = 0; // скидаємо стан клавіші
    }

    public void InitPlayMusicMain()
    {
        try
        {
            if (Play_Music_Main == null)
            {
                Play_Music_Main = new SoundPlayer("Music_MainTheme.wav"); // WAV замість MIDI, бо c# не підтримує
                Play_Music_Main.Load();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void StartPlayMusicMain()
    {
        try
        {
            if (Play_Music_Main != null && !muted)
            {
                Play_Music_Main.PlayLooping();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void SetMutedBgMusicMain(bool muted)
    {
        this.muted = muted;
        if (muted && Play_Music_Main != null)
        {
            Play_Music_Main.Stop();
        }
        else if (!muted)
        {
            StartPlayMusicMain();
        }
    }

    public void Start()
    {
        _isWorking = true;
        new Thread(Run).Start();
    }

    public void Stop()
    {
        _isWorking = false;
    }

    public void Run()
    {
        while (_isWorking)
        {
            Update(SleepTime);
            Invalidate();

            Thread.Sleep(SleepTime);
        }
    }

    public virtual void Update(long tick)
    {

    }

    /* protected virtual void Tick()
    {
        try
        {
            Thread.Sleep(mFrameDelay); // гарантує, що гра оновлюється з рівним інтервалом (16 мс) і не споживає 100% CPU
        }
        catch (ThreadInterruptedException) { }
        tick += mFrameDelay; // час проведений у грі
    } */ 

    protected virtual void Input(int key) { } // тут він порожній, але в нащадках можна перевизначати

    protected override void OnPaint(PaintEventArgs e) // малює кадр гри
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        Render(g);
    }

    protected virtual void Render(Graphics g) // очищає фон кольором
    {
        DrawBg(g);
    }

    protected void DrawBg(Graphics g) // задаєм колір очищення
    {
        g.Clear(Color.MidnightBlue);
    }

    /* public static void Debug(string message)
    {
        Console.WriteLine(message);
    } */

    public void Write(Graphics g, string msg, Color color, int x, int y)
    {
        using (Font font = new Font("Arial", 16, FontStyle.Bold))
        using (Brush brush = new SolidBrush(color))
        {
            g.DrawString(msg, font, brush, x, y);
        }
    }

    public void WriteRecord(string filePath, int value) // зберігає число у файл
    {
        try
        {
            File.WriteAllBytes(filePath, BitConverter.GetBytes(value));
        }
        catch (Exception e)
        {
            Console.WriteLine("File write error: " + e.Message);
        }
    }

    public int ReadRecord(string filePath) // читає число з файлу (якщо файл існує)
    {
        try
        {
            if (File.Exists(filePath))
            {
                byte[] data = File.ReadAllBytes(filePath);
                return BitConverter.ToInt32(data, 0);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("File read error: " + e.Message);
        }
        return 0;
    }
}