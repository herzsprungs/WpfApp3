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
    
    public partial class LibrarianAdminCourseCRUDpage : Page
    {
        private DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthVilleDatabase_by_RM_AND_JSConnectionString);

        public LibrarianAdminCourseCRUDpage()
        {
            InitializeComponent();
            LoadCourses();
            coursesDataGrid.CellEditEnding += CoursesDataGrid_CellEditEnding;
        }

        public class CourseDisplay
        {
            public string CourseID { get; set; } 
            public string Code { get; set; }
            public string Name { get; set; }
            public int StudentCount { get; set; }
            public string Department { get; set; }
            public string Status { get; set; }
        }

        private void LoadCourses()
        {
            var courses = db.Courses.Select(c => new CourseDisplay
            {
                CourseID = c.C_CourseID,
                Code = "COURSE-" + c.C_CourseID,
                Name = c.C_CourseName,
                StudentCount = db.NorthVilleStudents.Count(s => s.C_CourseID == c.C_CourseID),
                Department = "N/A", 
                Status = "Active"  
            }).ToList();

            coursesDataGrid.ItemsSource = courses;
        }

        private void AddCourse_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCourseCode.Text) && !string.IsNullOrWhiteSpace(txtCourseName.Text))
            {
                
                string courseCode = txtCourseCode.Text.Trim();
                if (courseCode.Length >= 2 && char.IsLetter(courseCode[0]) && int.TryParse(courseCode.Substring(1), out _))
                {
                    var course = new Course
                    {
                        C_CourseID = courseCode,  
                        C_CourseName = txtCourseName.Text.Trim()  
                    };

                    db.Courses.InsertOnSubmit(course);
                    db.SubmitChanges();
                   
                    MessageBox.Show("Course successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCourses();

                    txtCourseCode.Clear();
                    txtCourseName.Clear();
                }
                else
                {
                    MessageBox.Show("Please enter a valid Course Code (e.g., C021).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter both Course Code and Course Name.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CoursesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var editedItem = e.Row.Item as CourseDisplay;
                if (editedItem != null)
                {
                    var course = db.Courses.FirstOrDefault(c => c.C_CourseID == editedItem.CourseID);
                    if (course != null)
                    {
                        course.C_CourseName = editedItem.Name.Trim();
                        db.SubmitChanges();
                    }
                }
            }
        }

        private void DeleteCourse_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                string courseId = btn.Tag.ToString();
                var course = db.Courses.FirstOrDefault(c => c.C_CourseID == courseId);
                if (course != null)
                {
                    if (MessageBox.Show($"Are you sure you want to delete '{course.C_CourseName}'?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.Courses.DeleteOnSubmit(course);
                            db.SubmitChanges();
                            LoadCourses();
                        }
                        catch (System.Data.SqlClient.SqlException ex)
                        {
                            // Error code 547 = foreign key constraint violation in SQL Server
                            if (ex.Number == 547)
                            {
                                MessageBox.Show("Cannot delete this course because there are students enrolled in it.", "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                MessageBox.Show("An unexpected database error occurred:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An unexpected error occurred:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
    }
}
