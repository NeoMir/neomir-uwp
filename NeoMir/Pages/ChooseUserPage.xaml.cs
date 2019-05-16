using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace NeoMir.Pages
{
    public sealed partial class ChooseUserPage : Page
    {
        //
        // PROPERTIES
        //

        public static int Size = 250;
        public static int HoverSize = 20;
        public static Thickness BoxMargin = new Thickness(10);

        //
        // CONSTRUCTOR
        //

        public ChooseUserPage()
        {
            this.InitializeComponent();
            GetUsers();
            ListUsers(Users);
        }

        //
        // METHODS
        //

        private void GetUsers()
        {
            Classes.UserManager.Users.Add(new Classes.User("Robin", "DACALOR"));
            Classes.UserManager.Users.Add(new Classes.User("Ambroise", "DAMIER"));
            Classes.UserManager.Users.Add(new Classes.User("Martin", "BAUD"));
        }

        private void ListUsers(ItemsControl itemsControl)
        {
            int numberOfUsers = Classes.UserManager.Users.Count;

            for (int i = 0; i < numberOfUsers; i++)
            {
                TextBox textBox = new TextBox();

                textBox.Text = Classes.UserManager.Users[i].FirstName + ' ' + Classes.UserManager.Users[i].LastName;
                textBox.Height = Size;
                textBox.Width = Size;
                textBox.Margin = BoxMargin;
                textBox.TextAlignment = TextAlignment.Center;
                textBox.PointerEntered += new PointerEventHandler(textBox_PointerEntered);
                textBox.PointerExited += new PointerEventHandler(textBox_PointerExited);
                textBox.Tapped += new TappedEventHandler(textBox_Tapped);

                itemsControl.Items.Add(textBox);
            }

        }

        private void textBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBox img = (TextBox)sender;
            Classes.AppManager.GoToHome();
        }

        private void textBox_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            TextBox img = (TextBox)sender;
            img.Height += HoverSize;
            img.Width += HoverSize;
        }

        private void textBox_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            TextBox img = (TextBox)sender;
            img.Height -= HoverSize;
            img.Width -= HoverSize;
        }
    }
}
