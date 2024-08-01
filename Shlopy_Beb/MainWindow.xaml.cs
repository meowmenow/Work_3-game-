using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading; //Добавляем таймер

namespace Shlopy_Beb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Класс таймера с именем "Game Timer"
        DispatcherTimer gameTimer = new DispatcherTimer();
        //Гравитация содержит значение 8 (почти горизонт событий)
        int gravity = 8;
        //Счётчик очков
        double score;
        //Прямоугольник, который поможет нам обнаруживать коллизии
        Rect FlappyRect;
        //Значение для проверки, закончилась игра или нет 
        bool gameover = false;

        public MainWindow()
        {
            InitializeComponent();

            //Устанавливаем настройки по умолчанию для таймера
            gameTimer.Tick += gameEngine; //Связываем тик таймера с событием игрового движка
            gameTimer.Interval = TimeSpan.FromMilliseconds(20); //Устанавливаем интервал в 20 миллисекунд

            //Запускаем функцию запуска игры
            startGame();

        }


        private void Canvas_KeyisDown(object sender, KeyEventArgs e)
        {
            //Если нажата клавиша пробела, то
            if (e.Key == Key.Space)
            {
                //Повернуть изображение птицы на -20 градусов от центрального положения
                flappyBird.RenderTransform = new RotateTransform(-20, flappyBird.Width / 2, flappyBird.Height / 2);
                //Изменить гравитацию, чтобы наш птиц двигался вверх
                gravity = -8;
            }
            if (e.Key == Key.R && gameover == true)
            {
                //Если нажата клавиша "R" и для логического значения "game over" установлено значение "true"
                //Запускаем функцию запуска игры
                startGame();
            }
        }

        private void Canvas_KeyisUp(object sender, KeyEventArgs e)
        {
            //Если клавишу отпустить, то мы изменим поворот птички на 5 градусов от центра
            flappyBird.RenderTransform = new RotateTransform(5, flappyBird.Width / 2, flappyBird.Height / 2);
            //Становимся властелином гравитации, чтобы поменять её значение на 8 и птица полетела вниз, а не вверх (не суицид)
            gravity = 8;
        }

        private void startGame()
        {
            //Функция начала игры
            //Функция загрузит все значения по умолчанию, чтобы начать игру

            //Сначала создаем временное целое число со значением 200 (эффект параллакса, скорость облачков)
            int temp = 200;

            //Установить счёт равным 0
            score = 0;
            //Устанавливаем верхнее положение птички на 100 пикселей, т.е. на старт - внимание - марш
            Canvas.SetTop(flappyBird, 100);
            //Устанавливаем игру в false
            gameover = false;

            //Приведенный ниже цикл проверит каждое изображение в игре и установит их позиции по умолчанию
            foreach (var x in MyCanvas.Children.OfType<Image>())
            {
                //Устанавливаем каналы трубешника1 в положение по умолчанию
                if (x is Image && (string)x.Tag == "obs1")
                {
                    Canvas.SetLeft(x, 500);
                }
                //Устанавливаем каналы трубешника2 в положение по умолчанию
                if (x is Image && (string)x.Tag == "obs2")
                {
                    Canvas.SetLeft(x, 800);
                }
                //Устанавливаем каналы трубешника3 в положение по умолчанию
                if (x is Image && (string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x, 1000);
                }
                //Устанавливаем облака в положение по умолчанию
                if (x is Image && (string)x.Tag == "clouds")
                {
                    Canvas.SetLeft(x, (300 + temp));
                    temp = 800;
                }

            }
            //Запускаем основной игровой таймер
            gameTimer.Start();

        }

        private void gameEngine(object sender, EventArgs e)
        {
            //Это событие игрового движка, связанное с таймером

            //Обновляем текстовую метку счета с целым числом очков
            scoreText.Content = "Score: " + score;

            //Задаём хитбоксы птички
            FlappyRect = new Rect(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width - 12, flappyBird.Height - 6);

            //Установить гравитацию для изображения летающей пташки на холсте
            Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + gravity);

            //Проверяем, не вышла ли птица за пределы экрана сверху или снизу
            if (Canvas.GetTop(flappyBird) + flappyBird.Height > 490 || Canvas.GetTop(flappyBird) < -30)
            {
                //Если да, то "вы погибли" давай по новой
                gameTimer.Stop();
                gameover = true;
                scoreText.Content += "   Press R to Try Again";
            }

            //Ниже основной цикл, который будет проходить через каждое изображение на холсте
            //Если он находит любое изображение с тегами, следуем инструкции

            foreach (var x in MyCanvas.Children.OfType<Image>())
            {
                if ((string)x.Tag == "obs1" || (string)x.Tag == "obs2" || (string)x.Tag == "obs3")
                {

                    //Если мы нашли изображение с тегом obs1,2 или 3, то мы переместим его влево

                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);

                    //Задаём хитбоксы труб
                    Rect pillars = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    //Авария 
                    if (FlappyRect.IntersectsWith(pillars))
                    {
                        //Остановить таймер, установить игру в "true" и показать текст сброса (с крыши)
                        gameTimer.Stop();
                        gameover = true;
                        scoreText.Content += "   Press R to Try Again";
                    }

                }

                //Если первая труба покидает сцену и уходит на -100 пикселей слева
                //Нам нужно сбросить трубу, чтобы вернуться снова
                if ((string)x.Tag == "obs1" && Canvas.GetLeft(x) < -100)
                {
                    // сбрасываем канал до 800 пикселей
                    Canvas.SetLeft(x, 800);
                    // добавляем 1 к счету
                    score = score + .5;

                }
                //Если вторая труба покидает сцену и уходит на -200 пикселей слева
                if ((string)x.Tag == "obs2" && Canvas.GetLeft(x) < -200)
                {
                    //Мы устанавливаем эту трубу на 800 пикселей
                    Canvas.SetLeft(x, 800);
                    //Добавляем 1 к счету
                    score = score + .5;
                }
                //Если третий слой труб покидает сцену и уходит на -250 пикселей слева
                if ((string)x.Tag == "obs3" && Canvas.GetLeft(x) < -250)
                {
                    //Мы устанавливаем трубу на 800 пикселей
                    Canvas.SetLeft(x, 800);
                    //Добавляем 1 к счету
                    score = score + .5;

                }

                //Двигаем облока белогривые лошадкииии облокааааа
                if ((string)x.Tag == "clouds")
                {
                    //Затем мы будем медленно перемещать облако влево от экрана
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - .6);


                    //Если облака достигли -220 пикселей, то мы сбросим их
                    if (Canvas.GetLeft(x) < -220)
                    {
                        //Сбрасываем изображения облаков до 550 пикселей
                        Canvas.SetLeft(x, 550);
                    }
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e) //Выход на кнопку "Esc"
        {
            this.Close(); 
        }
    }
}

