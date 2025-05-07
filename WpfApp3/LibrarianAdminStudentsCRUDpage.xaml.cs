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

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for LibrarianAdminStudentsCRUDpage.xaml
    /// </summary>
    public partial class LibrarianAdminStudentsCRUDpage : Page
    {
        private DataClasses1DataContext db = new DataClasses1DataContext();
        private NorthVilleStudent _selectedStudent;

        public LibrarianAdminStudentsCRUDpage()
        {
            InitializeComponent();
            LoadStudents();
            LoadCourses();
        }

        public class StudentDisplay
        {
            public string N_StudentID { get; set; }
            public string N_FirstName { get; set; }
            public string N_LastName { get; set; }
            public string N_Email { get; set; }
            public string N_PhoneNumber { get; set; }
            public string C_CourseID { get; set; }
        }

        private void LoadStudents()
        {
            var students = db.NorthVilleStudents
                .Select(s => new StudentDisplay
                {
                    N_StudentID = s.N_StudentID,
                    N_FirstName = s.N_FirstName,
                    N_LastName = s.N_LastName,
                    N_Email = s.N_Email,
                    N_PhoneNumber = s.N_PhoneNumber,
                    C_CourseID = s.C_CourseID
                }).ToList();

            studentsDataGrid.ItemsSource = students;
        }

        private void LoadCourses()
        {
            var courses = db.Courses
                .Select(c => new { c.C_CourseID, c.C_CourseName })
                .ToList();

            cmbCourse.ItemsSource = courses;
            cmbCourse.DisplayMemberPath = "C_CourseName";
            cmbCourse.SelectedValuePath = "C_CourseID";
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentID.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                cmbCourse.SelectedValue == null)
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            var newStudent = new NorthVilleStudent
            {
                N_StudentID = txtStudentID.Text.Trim(),
                N_FirstName = txtFirstName.Text.Trim(),
                N_LastName = txtLastName.Text.Trim(),
                N_Email = txtEmail.Text.Trim(),
                N_PhoneNumber = txtPhone.Text.Trim(),
                C_CourseID = cmbCourse.SelectedValue.ToString()
            };

            try
            {
                db.NorthVilleStudents.InsertOnSubmit(newStudent);
                db.SubmitChanges();
                LoadStudents();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding student: {ex.Message}");
            }
        }

        private void EditStudent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                string studentId = btn.Tag.ToString();
                _selectedStudent = db.NorthVilleStudents.FirstOrDefault(s => s.N_StudentID == studentId);

                if (_selectedStudent != null)
                {
                    txtStudentID.Text = _selectedStudent.N_StudentID;
                    txtFirstName.Text = _selectedStudent.N_FirstName;
                    txtLastName.Text = _selectedStudent.N_LastName;
                    txtEmail.Text = _selectedStudent.N_Email;
                    txtPhone.Text = _selectedStudent.N_PhoneNumber;
                    cmbCourse.SelectedValue = _selectedStudent.C_CourseID;

                    MessageBox.Show($"Editing student: {_selectedStudent.N_StudentID}");
                }
            }
        }

        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                string studentId = btn.Tag.ToString();
                var student = db.NorthVilleStudents.FirstOrDefault(s => s.N_StudentID == studentId);

                if (student != null && MessageBox.Show($"Delete student {student.N_StudentID}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    db.NorthVilleStudents.DeleteOnSubmit(student);
                    db.SubmitChanges();
                    LoadStudents();
                }
            }
        }

        private void StudentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStudent = studentsDataGrid.SelectedItem as NorthVilleStudent;
        }

        private void ClearForm()
        {
            txtStudentID.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            cmbCourse.SelectedIndex = -1;
        }
    }
}
