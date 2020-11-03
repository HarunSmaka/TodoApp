using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TodoApp
{
    public partial class Form1 : Form
    {
        private readonly DataAccess dataAccessManager;
        User user = new User();
        List<Todos> todos = new List<Todos>();
        List<DoneTodos> doneTodos = new List<DoneTodos>();

        public Form1()
        {
            InitializeComponent();
            dataAccessManager = new DataAccess(Helper.GetConnectionString("TodoApp"),Helper.GetConnectionString("Master"), "todoApp");

            Todo.Appearance = TabAppearance.FlatButtons;
            Todo.ItemSize = new Size(0, 1);
            Todo.SizeMode = TabSizeMode.Fixed;
        }

        private void UpdateTodos(DataAccess dataAccess)
        {
            todos = dataAccess.GetTodos(user.Email);
            todoListBox.DataSource = todos;
            todoListBox.DisplayMember = "Content";

        }
        private void UpdateDoneTodos(DataAccess dataAccess)
        {
            doneTodos = dataAccess.GetDoneTodos(user.ID);
            doneTodoListBox.DataSource = doneTodos;
            doneTodoListBox.DisplayMember = "Content";

        }

        private void ShowPasswordL_Click(object sender, EventArgs e)
        {
            PasswordTextBoxL.UseSystemPasswordChar = !PasswordTextBoxL.UseSystemPasswordChar;
        }

        private void ShowPasswordS_Click(object sender, EventArgs e)
        {
            PasswordTextBoxS.UseSystemPasswordChar = !PasswordTextBoxS.UseSystemPasswordChar;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {            
            user = dataAccessManager.GetUser(EmailTextBoxL.Text, PasswordTextBoxL.Text);
            if (user == null)
            {
                MessageBox.Show("Check your username and password");
                return;
            }

            EmailTextBoxL.Text = String.Empty;
            PasswordTextBoxL.Text = String.Empty;
            UserLabel.Text = user.FirstName + " " + user.LastName;
            Todo.SelectedTab = mainPage;

            UpdateTodos(dataAccessManager);

        }

        private void SignUpLabelL_Click(object sender, EventArgs e)
        {
            Todo.SelectedTab = signUpPage;
        }

        private void LogInLabelS_Click(object sender, EventArgs e)
        {
            Todo.SelectedTab = loginPage;

            FNameTextBoxS.Text = String.Empty;
            LNameTextBoxS.Text = String.Empty;
            EmailTextBoxS.Text = String.Empty;
            PasswordTextBoxS.Text = String.Empty;
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            Todo.SelectedTab = loginPage;
            todoListBox.DataSource = null;
            todoListBox.Items.Clear();

            todos.Clear();
            doneTodos.Clear();
        }

        private void SignUpButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataAccessManager.EmailSearch(EmailTextBoxS.Text) != 0)
                {
                    MessageBox.Show("User with this email already exists.");
                    return;
                }

                dataAccessManager.InsertUser(FNameTextBoxS.Text, LNameTextBoxS.Text, EmailTextBoxS.Text, PasswordTextBoxS.Text);
                user = dataAccessManager.GetUser(EmailTextBoxS.Text, PasswordTextBoxS.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to create user.");
                return;
            }

            UserLabel.Text = FNameTextBoxS.Text.Trim() + " " + LNameTextBoxS.Text.Trim();

            FNameTextBoxS.Clear();
            LNameTextBoxS.Clear();
            EmailTextBoxS.Clear();
            PasswordTextBoxS.Clear();

            Todo.SelectedTab = mainPage;
        }

        private void AddTodoButtonM_Click(object sender, EventArgs e)
        {
            if (AddTodoTextBoxM.Text == String.Empty)
            {
                MessageBox.Show("Text box is empty");
                return;
            }

            dataAccessManager.InsertTodo(AddTodoTextBoxM.Text, user.ID);
            AddTodoTextBoxM.Text = String.Empty;

            UpdateTodos(dataAccessManager);
        }

        private void DeleteButtonM_Click(object sender, EventArgs e)
        {
            string selectedTodo = todoListBox.GetItemText(todoListBox.SelectedItem);

            dataAccessManager.DeleteTodo(selectedTodo, user.ID);
            UpdateTodos(dataAccessManager);
        }

        private void DoneTodosButtonM_Click(object sender, EventArgs e)
        {
            Todo.SelectedTab = deleteTodoTab;
            UserLabelD.Text = user.FirstName + " " + user.LastName;

            UpdateDoneTodos(dataAccessManager);
        }

        private void TodosButtonD_Click(object sender, EventArgs e)
        {
            Todo.SelectedTab = mainPage;
        }

        private void MarkAsDoneButtonM_Click(object sender, EventArgs e)
        {
            string selectedTodo = todoListBox.GetItemText(todoListBox.SelectedItem);

            dataAccessManager.InsertDoneTodo(selectedTodo, user.ID);
            dataAccessManager.DeleteTodo(selectedTodo, user.ID);

            UpdateTodos(dataAccessManager);
        }

        private void deleteTodoButtonD_Click(object sender, EventArgs e)
        {
            string selectedTodo = doneTodoListBox.GetItemText(doneTodoListBox.SelectedItem);

            dataAccessManager.DeleteDoneTodo(selectedTodo, user.ID);
            UpdateDoneTodos(dataAccessManager);
        }

        private void deleteAllButtonD_Click(object sender, EventArgs e)
        {
            dataAccessManager.DeleteAllDoneTodos();
            UpdateDoneTodos(dataAccessManager);
        }

        private void LogOutButtonD_Click(object sender, EventArgs e)
        {
            Todo.SelectedTab = loginPage;
        }

        private void EditButtonM_Click(object sender, EventArgs e)
        {
            string selectedTodo = todoListBox.GetItemText(todoListBox.SelectedItem);
            string todoForEdit = Interaction.InputBox("Edit todo: ", "Edit todo", selectedTodo, 500, 300);

            if (todoForEdit == String.Empty)
            {
                dataAccessManager.DeleteTodo(selectedTodo, user.ID);
                UpdateTodos(dataAccessManager);
                return;
            }

            dataAccessManager.UpdateTodo(todoForEdit, selectedTodo, user.ID);
            UpdateTodos(dataAccessManager);

        }
    }
}
