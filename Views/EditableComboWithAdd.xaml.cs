using System.Windows;
using System.Windows.Controls;

namespace Facturon.App.Views
{
    public partial class EditableComboWithAdd : UserControl
    {
        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
            nameof(DisplayMemberPath), typeof(string), typeof(EditableComboWithAdd), new PropertyMetadata(null));

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public EditableComboWithAdd()
        {
            InitializeComponent();
        }

        public void FocusInput()
        {
            Combo.Focus();
        }
    }
}
