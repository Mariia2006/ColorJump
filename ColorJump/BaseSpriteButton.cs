using System;
using System.Drawing;
using System.Windows.Forms;

public class BaseSpriteButton // створює кнопку для граф інтерфейсу з використанням зображень
{
    public int Top { get; private set; }
    public int Left { get; private set; }
    public int Height { get; private set; }
    public int Width { get; private set; }
    private int numPart; // змінна, яка зберігає кількість частин спрайту
    private Bitmap img; // змінна, що зберігає зображення спрайту
    private bool isPressed; // змінна, яка вказує, чи кнопка натиснута

    public bool IsPressed // властивість, щоб визначити натиснута кнопка чи ні
    {
        get { return isPressed; }
        set { isPressed = value; }
    }

    public BaseSpriteButton(string imgPath, int numPart, bool press) //  конструктор, який ініціалізує кнопку з шляхом до зображення
    {
        this.numPart = numPart;
        isPressed = press;

        if (!string.IsNullOrEmpty(imgPath))
        {
            try
            {
                img = new Bitmap(imgPath);
                Height = img.Height;
                Width = img.Width / numPart;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public BaseSpriteButton(int x, int y, string imgPath, int numPart, bool press): this(imgPath, numPart, press) // перевантажений конструктор, який дозволяє додатково вказати початкову позицію кнопки
    {
        SetPosition(x, y);
    }

    public void SetPosition(int x, int y) // метод для встановлення позиції кнопки на екрані
    {
        Left = x;
        Top = y;
    }

    public bool IsTapOnButton(int x, int y) // метод для перевірки, чи потрапляє клік миші в область кнопки
    {
        return x > Left && x < Left + Width && y > Top && y < Top + Height;
    }

    public void DrawButton(Graphics g) // метод для малювання кнопки на екрані
    {
        if (img == null) return;

        int frameIndex = isPressed ? numPart - 1 : 0;
        Rectangle sourceRect = new Rectangle(frameIndex * Width, 0, Width, Height);
        Rectangle destRect = new Rectangle(Left, Top, Width, Height);

        g.DrawImage(img, destRect, sourceRect, GraphicsUnit.Pixel);
    }
}

