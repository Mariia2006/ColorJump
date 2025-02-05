using System.Drawing;

public class BaseButton
{
    public int Top { get; set; } // координати точки на екрані
    public int Left { get; set; } 
    public int Height { get; private set; } // розміри кнопки
    public int Width { get; private set; }
    public Image PressImg { get; private set; } // зображення кнопки коли вона натиснута
    public Image IdleImg { get; private set; } // зображення коли не натиснута
    public bool IsPressed { get; set; } // зберігає стан кнопки

    public BaseButton(int x, int y, string idleImgStr, string pressImgStr, bool press)
    {
        Left = x; // встановлюємо координати точки
        Top = y;
        PressImg = LoadImage(pressImgStr); // завантажуємо зображення кнопки
        IdleImg = LoadImage(idleImgStr);

        Height = IdleImg?.Height ?? 0; // визначаємо висоту та ширину кнопки за розміром IdleImg
        Width = IdleImg?.Width ?? 0;
        IsPressed = press; // встановлюємо початковий стан кнопки
    }

    public BaseButton(string idleImgStr, string pressImgStr, bool press) // конструктор без координат точки
    {
        PressImg = LoadImage(pressImgStr);
        IdleImg = LoadImage(idleImgStr);

        Height = IdleImg?.Height ?? 0;
        Width = IdleImg?.Width ?? 0;
        IsPressed = press;
    }

    public bool IsTapOnButton(int x, int y)
    {
        return x > Left && x < Left + Width && y > Top && y < Top + Height; // перевіряє, чи точка в межах розміру кнопки
    }

    public void DrawButton(Graphics g)
    {
        if (IsPressed && PressImg != null) // якщо кнопка натиснута, малюємо натиснуте зображення
        {
            g.DrawImage(PressImg, Left, Top);
        }
        else if (IdleImg != null) // якщо кнопка не натиснута, малюємо IdleImg
        {
            g.DrawImage(IdleImg, Left, Top);
        }
    }

    public void DrawButton(Graphics g, int x, int y) // перевантаження, що дозволяє намалювати кнопку в іншому місці
    {
        Left = x;
        Top = y;
        DrawButton(g);
    }

    public void SetPosition(int left, int top) // встановлює нові координати кнопки
    {
        Top = top;
        Left = left;
    }

    private Image LoadImage(string path) // завантаження зображення з файлу
    { // * зробити класом
        if (string.IsNullOrEmpty(path)) return null;

        try
        {
            return Image.FromFile(path);
        }
        catch
        {
            return null;
        }
    }
}