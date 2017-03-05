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
using System.Windows.Threading;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CLEANER_TABLE
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //自定义变量区
        //===================

        //开始打扫的时间(年，月，日)
        static DateTime timeStart = new DateTime(2014, 9, 1);

        //宿舍人数
        static int pCount = 4;

        //宿舍成员信息（姓名，学号，介绍，图片地址），没有值就用空字符串
        static Menber[] member = {
            new Menber("陈萍",  1,"我是陈萍",@"E:\Course\C#\值日表\WpfApplication1\image\2.jpg",""),
            new Menber("钱宇彤",2,"我是钱宇彤",@"E:\Course\C#\值日表\WpfApplication1\image\2.jpg",""),
            new Menber("陶欢欢",3,"我是陶欢欢",@"E:\Course\C#\值日表\WpfApplication1\image\2.jpg","D:\\CloudMusic\\Sou - メリュー.mp3"),
            new Menber("黄诗雨",4,"我是御正",@"E:\Course\C#\值日表\WpfApplication1\image\2.jpg","D:\\CloudMusic\\Nao'ymt - Lil' Goldfish.mp3")
        };

        //====================

        static bool isMax = false;
        static bool isSideBarDispaly = false;
        static Style iniStyle = new Style();
        static Button duty;

        public class Style
        {
            public int col, row, colSpan, rowSpan;
            public Brush bg;
        }

        public class Menber
        {
            public string name, intro, picture, sound;
            public int num;

            public Menber(string name, int num, string intro, string picture, string sound)
            {
                this.name = name;
                this.num = num;
                this.intro = intro;
                this.picture = picture;
                this.sound = sound;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            //初始化当前时间
            label5.Content = DateTime.Now.ToLongTimeString().ToString();
            label6.Content = DateTime.Now.ToString("yyyy-MM-dd");

            //计时器
            DispatcherTimer time = new DispatcherTimer();
            time.Interval = new TimeSpan(0, 0, 1);
            time.Tick += new EventHandler(setTime);
            time.Start();

            //计算今天是谁打扫
            TimeSpan dayNow = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan dayStart = new TimeSpan(timeStart.Ticks);
            int timeDiff = dayNow.Subtract(dayStart).Duration().Days;

            //遍历按钮插值
            for (int i = 0; i < member.Length; i++)
            {
                string btnName = "button" + i,
                       imgName = "image" + i,
                       labelName = "label" + i,
                       textBlockName = "textBlock" + i,
                       titleName = "title" + i;

                var btn = GetControlObject<Button>(btnName);

                btn.Opacity = 0.5;
                if (member[i].num == timeDiff % pCount + 1)
                {
                    btn.Opacity = 1;
                    duty = btn;
                    label7.Content = "今天是" + member[i].name + "值日";
                    btn.Opacity = 1;
                }

                if (member[i].picture != "")
                {
                    try
                    {
                        var imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), member[i].picture)));
                        imgBrush.Stretch = Stretch.UniformToFill;
                        btn.Background = imgBrush;
                        GetControlObject<Image>(imgName).Source = new BitmapImage(new Uri(member[i].picture, UriKind.Absolute));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                GetControlObject<Label>(labelName).Content = member[i].name;
                GetControlObject<TextBlock>(textBlockName).Text = member[i].intro;
                GetControlObject<Label>(titleName).Content = member[i].name;


            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            Button btn = (Button)sender;
            string index = getIndex(btn.Name);
            string gridName = "grid" + index;
            string titleName = "title" + index;
            Grid grid = GetControlObject<Grid>(gridName);
            Label title = GetControlObject<Label>(titleName);

            if (!isMax)
            {
                iniStyle.col = Grid.GetColumn(btn);
                iniStyle.row = Grid.GetRow(btn);
                iniStyle.colSpan = Grid.GetColumnSpan(btn);
                iniStyle.rowSpan = Grid.GetRowSpan(btn);
                iniStyle.bg = btn.Background;

                Panel.SetZIndex(btn, 3);

                Grid.SetColumn(btn, 0);
                Grid.SetRow(btn, 0);
                Grid.SetColumnSpan(btn, 3);
                Grid.SetRowSpan(btn, 2);

                if (grid is Grid && grid != null) grid.Visibility = Visibility.Visible;
                button.Visibility = Visibility.Visible;

                try
                {
                    title.Visibility = Visibility.Hidden;
                }
                catch (Exception ev)
                {
                    Console.WriteLine(ev.Message);
                }

                btn.Background = btn.Background is ImageBrush ? (Brush)new BrushConverter().ConvertFrom("#FF6D869E") : btn.Background;
                btn.Opacity = 1;

                int sIndex = Convert.ToInt32(index);
                if (sIndex < member.Length)
                {
                    string src = member[sIndex].sound;
                    if (src != "")
                    {
                        mediaElement.Source = new Uri(src);
                        mediaElement.Play();
                    }
                }

                isMax = true;
            }
            else
            {
                Grid.SetColumn(btn, iniStyle.col);
                Grid.SetRow(btn, iniStyle.row);
                Grid.SetColumnSpan(btn, iniStyle.colSpan);
                Grid.SetRowSpan(btn, iniStyle.rowSpan);
                if (grid is Grid && grid != null) grid.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
                sideBar.Visibility = Visibility.Hidden;

                if (btn != duty)
                {
                    btn.Opacity = 0.5;
                }
                try
                {
                    title.Visibility = Visibility.Visible;
                }
                catch { }
                btn.Background = iniStyle.bg;

                mediaElement.Pause();
                Panel.SetZIndex(btn, 1);

                isMax = false;
            }
        }

        //Get index
        private string getIndex(string str)
        {
            Regex regex = new Regex(@"\d");
            string index = regex.Match(str).Groups[0].Value;
            return index;
        }

        //初始化按钮
        /// <summary>
        /// 根据控件的Name获取控件对象
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="controlName">Name</param>
        /// <returns></returns>
        /// http://blog.csdn.net/mking69/article/details/52527164
        /// 
        public T GetControlObject<T>(string controlName)
        {
            try
            {
                Type type = this.GetType();
                FieldInfo fieldInfo = type.GetField(controlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                if (fieldInfo != null)
                {
                    T obj = (T)fieldInfo.GetValue(this);
                    return obj;
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        //初始化时间
        public void setTime(object source, EventArgs e)
        {
            try
            {
                label5.Content = DateTime.Now.ToLongTimeString().ToString();
                label6.Content = DateTime.Now.ToString("yyyy-MM-dd");
            }
            catch (Exception ev)
            {
                Console.WriteLine(ev.ToString());
            }
        }

        private void sidebar(object sender, RoutedEventArgs e)
        {
            if (isSideBarDispaly)
            {
                sideBar.Visibility = Visibility.Hidden;
                button.Content = "≡";
            }
            else
            {
                sideBar.Visibility = Visibility.Visible;
                button.Content = "×";
            }
            isSideBarDispaly = !isSideBarDispaly;
        }

        private void sideBarClose(object sender, RoutedEventArgs e)
        {
            sideBar.Visibility = Visibility.Hidden;
        }

        //sidebar按钮的hover效果
        private void button_Copy3_MouseEnter(object sender, MouseEventArgs e)
        {
            button.Visibility = Visibility.Visible;
        }

        private void button_Copy3_MouseLeave(object sender, MouseEventArgs e)
        {
            button.Visibility = Visibility.Hidden;
        }

        private void closeWindow(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ev)
            {
                Console.WriteLine(ev.Message);
            }
        }

        private void pause(object sender, RoutedEventArgs e)
        {
            if (isMax && mediaElement.Source != null)
            {
                ((Button)sender).Content = mediaElement.IsMuted ? "暂停" : "播放";
                mediaElement.IsMuted = !mediaElement.IsMuted;
            }
        }

        //容器按钮的hover效果

        private void button3_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Opacity = 1;
        }

        private void button3_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isMax)
            {
                var btn = ((Button)sender);
                if (btn != duty) btn.Opacity = 0.5;
            }
        }
    }
}
