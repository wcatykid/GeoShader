using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DynamicGeometry.UI.GivenWindow;
using GeometryTutorLib.EngineUIBridge;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib;
using LiveGeometry.TutorParser;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A window that allows the user to receive hints and enter solutions.
    /// </summary>
    public class EnterSolutionWindow : ChildWindow
    {
        public DrawingHost drawingHost { get; set; }

        private Dictionary<string, GroundedClause> currentSolution;
        private Dictionary<string, AddGivenWindow> givenWindows;
        private Dictionary<string, JustificationSwitch.DeductionJustType> justWindow;
        private List<string> currentJust;
        private List<CheckBox> affectedCheckboxes = new List<CheckBox>();
        private ListBox solutionList;
        private ListBox justList;
        private ListBox hintList;
        private TextBox problemBox;
        private TextBlock checkSolutionResult;
        private ComboBox addSolution;
        private ComboBox addJustification;
        public GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> problem { get; set; }
        private string problemString;
        HypergraphWrapper wrapper = null;
         
        public void SendProblem(
            HypergraphWrapper wrapper,
            GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> p,
            string probStr)
        {
            this.wrapper = wrapper;
            problem = p;
            problemString = probStr;
        }



        /// <summary>
        /// Create the window.
        /// </summary>
        public EnterSolutionWindow()
        {
            problem = null;
            Initialize();
            MakeSolutions();
            MakeJustifications();
            LayoutDesign();
        }

        ////
        //// When displaying the problem in the textbox, only display the source and goal; not solution path.
        ////
        //public void DisplayProblem()
        //{
        //    string probString = problem.ConstructProblemAndSolution();
        //}


        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Enter Solution Window";
            currentSolution = new Dictionary<string, GroundedClause>();
            currentJust = new List<string>();
            this.MaxHeight = 800;
            this.MaxWidth = 2000;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {



            //Set up grid
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });


            //Set up list of current solutions
            solutionList = new ListBox();
            solutionList.SelectionMode = SelectionMode.Extended;
            solutionList.ItemsSource = currentSolution.Keys;
            solutionList.MinHeight = 150;
            solutionList.MaxHeight = 150;
            solutionList.MinWidth = 350;
            solutionList.MaxWidth = 350;
            solutionList.Margin = new Thickness(0, 0, 5, 5);

            //Set up list of current justifications
            justList = new ListBox();
            justList.SelectionMode = SelectionMode.Extended;
            justList.ItemsSource = currentJust;
            justList.MinHeight = 150;
            justList.MaxHeight = 150;
            justList.MinWidth = 400;
            justList.MaxWidth = 400;
            justList.Margin = new Thickness(0, 0, 5, 5);

            //Set up hint list title text block
            TextBlock hint = new TextBlock();
            hint.Text = "Hint:";
            hint.Margin = new Thickness(0, 10, 5, 0);
            hint.TextAlignment = TextAlignment.Left;

            //Set up list of current hints
            hintList = new ListBox();
            hintList.SelectionMode = SelectionMode.Extended;
            hintList.ItemsSource = currentSolution.Keys;
            hintList.MinHeight = 150;
            hintList.MaxHeight = 150;
            hintList.MinWidth = 350;
            hintList.MaxWidth = 350;
            hintList.Margin = new Thickness(0, 0, 5, 5);

            //Set up problem box title text block
            TextBlock currentProblem = new TextBlock();
            currentProblem.Text = "Current Problem:";
            currentProblem.Margin = new Thickness(0, 10, 5, 0);
            currentProblem.TextAlignment = TextAlignment.Left;

            //Set up problem textbox
            problemBox = new TextBox();
            problemBox.MinHeight = 150;
            problemBox.MaxHeight = 150;
            problemBox.MinWidth = 400;
            problemBox.MaxWidth = 400;
            problemBox.Margin = new Thickness(0, 0, 5, 5);
            problemBox.IsEnabled = false;

            //Set up add and remove panel
            StackPanel addRemPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            addSolution = new ComboBox();
            addSolution.ItemsSource = givenWindows.Keys;
            addSolution.SelectedItem = addSolution.Items[0];
            addSolution.Margin = new Thickness(0, 5, 0, 5);
            addSolution.MinWidth = 293;
            addSolution.MaxWidth = 293;
            addSolution.MinHeight = 25;
            addSolution.MaxHeight = 25;
            addRemPanel.Children.Add(addSolution);
            Button addBtn = new Button();
            addBtn.Content = "+";
            addBtn.Click += new RoutedEventHandler(AddSolutionBtn_Click);
            addBtn.Margin = new Thickness(2, 5, 0, 5);
            addBtn.MinWidth = 25;
            addBtn.MinHeight = 25;
            addBtn.MaxHeight = 25;
            addRemPanel.Children.Add(addBtn);
            Button remBtn = new Button();
            remBtn.Content = "-";
            remBtn.Click += new RoutedEventHandler(RemoveSolutionBtn_Click);
            remBtn.Margin = new Thickness(2, 5, 0, 5);
            remBtn.MinWidth = 25;
            remBtn.MinHeight = 25;
            remBtn.MaxHeight = 25;
            addRemPanel.Children.Add(remBtn);

            //Set up justification add and remove panel
            StackPanel justAddPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            addJustification = new ComboBox();
            addJustification.ItemsSource = justWindow.Keys;
            addJustification.SelectedItem = addJustification.Items[0];
            addJustification.Margin = new Thickness(0, 5, 0, 5);
            addJustification.MinWidth = 343;
            addJustification.MaxWidth = 343;
            addJustification.MinHeight = 25;
            addJustification.MaxHeight = 25;
            justAddPanel.Children.Add(addJustification);
            Button justAddBtn = new Button();
            justAddBtn.Content = "+";
            justAddBtn.Click += new RoutedEventHandler(AddJustificationBtn_Click);
            justAddBtn.Margin = new Thickness(2, 5, 0, 5);
            justAddBtn.MinWidth = 25;
            justAddBtn.MinHeight = 25;
            justAddBtn.MaxHeight = 25;
            justAddPanel.Children.Add(justAddBtn);
            Button justRemBtn = new Button();
            justRemBtn.Content = "-";
            justRemBtn.Click += new RoutedEventHandler(RemoveJustificationBtn_Click);
            justRemBtn.Margin = new Thickness(2, 5, 0, 5);
            justRemBtn.MinWidth = 25;
            justRemBtn.MinHeight = 25;
            justRemBtn.MaxHeight = 25;
            justAddPanel.Children.Add(justRemBtn);

            //Solution reslult textblock
            checkSolutionResult = new TextBlock();
            checkSolutionResult.Text = "";
            checkSolutionResult.VerticalAlignment = VerticalAlignment.Center;
            checkSolutionResult.HorizontalAlignment = HorizontalAlignment.Center;

            // Create a check solution button
            Button checkSolutionBtn = new Button();
            checkSolutionBtn.Content = "Check Solution";
            checkSolutionBtn.Margin = new Thickness(7, 5, 7, 5);
            checkSolutionBtn.Width = 95;
            checkSolutionBtn.Height = 25;
            checkSolutionBtn.HorizontalAlignment = HorizontalAlignment.Center;
            checkSolutionBtn.Click += new RoutedEventHandler(CheckSolutionBtn_Click);

            // Create a submit button
            Button subBtn = new Button();
            subBtn.Content = "Submit";
            subBtn.Margin = new Thickness(0, 0, 10, 5);
            subBtn.Width = 95;
            subBtn.Height = 25;
            subBtn.HorizontalAlignment = HorizontalAlignment.Right;

            // Create a hint button
            Button hintBtn = new Button();
            hintBtn.Content = "Get Hint";
            hintBtn.Click += new RoutedEventHandler(HintBtn_Click);
            hintBtn.Margin = new Thickness(0, 0, 10, 5);
            hintBtn.Width = 95;
            hintBtn.Height = 25;
            hintBtn.HorizontalAlignment = HorizontalAlignment.Right;

            //Set element locations in grid and add to grid
            // 01
            Grid.SetColumn(solutionList, 0);
            Grid.SetRow(solutionList, 1);
            grid.Children.Add(solutionList);
            // 02
            Grid.SetColumn(addRemPanel, 0);
            Grid.SetRow(addRemPanel, 2);
            grid.Children.Add(addRemPanel);
            // 03
            Grid.SetColumn(hint, 0);
            Grid.SetRow(hint, 3);
            grid.Children.Add(hint);
            // 04
            Grid.SetColumn(hintList, 0);
            Grid.SetRow(hintList, 4);
            grid.Children.Add(hintList);
            // 05
            Grid.SetColumn(hintBtn, 0);
            Grid.SetRow(hintBtn, 5);
            grid.Children.Add(hintBtn);
            // 11
            Grid.SetColumn(justList, 1);
            Grid.SetRow(justList, 1);
            grid.Children.Add(justList);
            // 12
            Grid.SetColumn(justAddPanel, 1);
            Grid.SetRow(justAddPanel, 2);
            grid.Children.Add(justAddPanel);
            // 13
            Grid.SetColumn(currentProblem, 1);
            Grid.SetRow(currentProblem, 3);
            grid.Children.Add(currentProblem);
            //14
            Grid.SetColumn(problemBox, 1);
            Grid.SetRow(problemBox, 4);
            grid.Children.Add(problemBox);
            // 16

            // 22
            Grid.SetColumn(checkSolutionResult, 2);
            Grid.SetRow(checkSolutionResult, 1);
            grid.Children.Add(checkSolutionResult);

            Grid.SetColumn(checkSolutionBtn, 2);
            Grid.SetRow(checkSolutionBtn, 2);
            grid.Children.Add(checkSolutionBtn);
            // 23

            // 24

            // 25

            // 26
            Grid.SetColumn(subBtn, 2);
            Grid.SetRow(subBtn, 6);
            grid.Children.Add(subBtn);

            //Set the content of the window to be the newly designed layout
            this.Content = grid;
        }

        /// <summary>
        /// Create the add given windows and set up the list of solutions.
        /// </summary>
        private void MakeSolutions()
        {
            givenWindows = new Dictionary<string, AddGivenWindow>();
            givenWindows.Add("Congruent Segments", new AddCongruentSegments());
            //CTA: Need Midpoint
            givenWindows.Add("Congruent Angles", new AddCongruentAngles());
            givenWindows.Add("Segment Bisector", new AddSegmentBisector());
            givenWindows.Add("Angle Bisector", new AddAngleBisector());
            givenWindows.Add("Right Angle", new AddRightAngle());
            givenWindows.Add("Parallel Lines", new AddParallelLines());
            givenWindows.Add("Isosceles Triangle", new AddIsoscelesTriangle());
            givenWindows.Add("Equilateral Triangle", new AddEquilateralTriangle());
            givenWindows.Add("Congruent Triangles", new AddCongruentTriangles());
            givenWindows.Add("Similar Triangles", new AddSimilarTriangles());

            foreach (AddGivenWindow w in givenWindows.Values)
            {
                w.Closed += new EventHandler(AddGivenWindow_Close);
            }
        }


        /// <summary>
        /// This event executes when the "+" button is clicked.
        /// Will pop up a window so that the user can add the currently selected given in the combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSolutionBtn_Click(object sender, RoutedEventArgs e)
        {
            givenWindows[addSolution.SelectedValue as string].Show(new DrawingParserMain(drawingHost.CurrentDrawing), new List<GroundedClause>(currentSolution.Values));
        }

        /// <summary>
        /// This event executes when the "-" button is clicked.
        /// Will remove all selected items in the current givens list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSolutionBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (string selected in solutionList.SelectedItems)
            {
                currentSolution.Remove(selected);
            }

            //Graphically refresh the solution list.
            solutionList.ItemsSource = null;
            solutionList.ItemsSource = currentSolution.Keys;
        }


        /// <summary>
        /// This even executes when an add given window is closed.
        /// Will add the given if the window was accepted, and remeber the associated clause.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGivenWindow_Close(object sender, EventArgs e)
        {
            AddGivenWindow window = sender as AddGivenWindow;
            if (window.WindowResult == AddGivenWindow.Result.Accept)
            {
                GroundedClause clause = window.Clause;
                if (clause != null)
                {
                    currentSolution.Add(clause.ToPrettyString(), clause);
                }
            }

            //Graphically refresh the givens list.
            solutionList.ItemsSource = null;
            solutionList.ItemsSource = currentSolution.Keys;
        }

        /// <summary>
        /// This event executes when the "+" button is clicked.
        /// Will add the selected justification to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddJustificationBtn_Click(object sender, RoutedEventArgs e)
        {
            currentJust.Add(addJustification.SelectedValue as string);

            //Graphically refresh the solution list.
            justList.ItemsSource = null;
            justList.ItemsSource = currentJust;
        }

        /// <summary>
        /// This event executes when the "-" button is clicked.
        /// Will remove all selected items in the current justificationss list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveJustificationBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (string selected in justList.SelectedItems)
            {
                currentJust.Remove(selected);
            }

            //Graphically refresh the solution list.
            justList.ItemsSource = null;
            justList.ItemsSource = currentJust;
        }

        /// <summary>
        /// This event executes when the check solution button is clicked.
        /// Will perform a solution check and update the checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckSolutionBtn_Click(object sender, RoutedEventArgs e)
        {
            bool correctSolution = false;

            if (currentJust.Count > 0 && currentSolution.Values.Count > 0)
            {
                //System.Diagnostics.Debug.Assert(problem != null && wrapper != null);

                List<GroundedClause> solutions = new List<GroundedClause>(currentSolution.Values);
                List<JustificationSwitch.DeductionJustType> justifications = new List<JustificationSwitch.DeductionJustType>();
                currentJust.ForEach<String>(justStr => justifications.Add(justWindow[justStr]));

                var parser = new DrawingParserMain(drawingHost.CurrentDrawing);
                parser.Parse();


                //TODO: Perform checks here. Update the correctSolution variable to true if all checks pass.
                //Use solutions and justifications lists
                /* List<GroundedClause> passed = new List<GroundedClause>();
                 GroundedClause hint = null;
                 foreach (GroundedClause step in solutions)
                 {
                     if (wrapper.QueryNodeInGraph(step))
                     {
                         passed.Add(step);
                     }
                     else
                     {
                         hint = wrapper.QueryHint(problem, passed);
                         break;
                     }
                 }*/
            }
            
            //Provide feedback
            if (correctSolution)
            {
                checkSolutionResult.Text = "Correct!";
            }
            else
            {
                checkSolutionResult.Text = "Stuck?\nAsk for a hint.";
            }
        }

        private void HintBtn_Click(object sender, RoutedEventArgs e)
        {
            List<GroundedClause> solutions = new List<GroundedClause>(currentSolution.Values);
            var hint = new List<GroundedClause>();
            hint.Add(wrapper.QueryHint(problem, solutions));
            hintList.ItemsSource = null;
            hintList.ItemsSource = hint;
        }

        /// <summary>
        /// Create the add justification windows and set up the list of justifications.
        /// </summary>
        private void MakeJustifications()
        {
            //NOTE: Need unique strings! They are keys!
            //Also note that => is the math symbol for implies, so you can shorten strings with that
            justWindow = new Dictionary<string, JustificationSwitch.DeductionJustType>();
            //Axioms
            justWindow.Add("AA Similarity", JustificationSwitch.DeductionJustType.AA_SIMILARITY);
            justWindow.Add("Angle Addition Axiom", JustificationSwitch.DeductionJustType.ANGLE_ADDITION_AXIOM);
            justWindow.Add("ASA", JustificationSwitch.DeductionJustType.ASA);
            justWindow.Add("Congruent Corresponding Angles => Parallel", JustificationSwitch.DeductionJustType.CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL);
            justWindow.Add("Corresponding Angles of Parallel Lines", JustificationSwitch.DeductionJustType.CORRESPONDING_ANGLES_OF_PARALLEL_LINES);
            justWindow.Add("Segment Addition Axiom", JustificationSwitch.DeductionJustType.SEGMENT_ADDITION_AXIOM);
            justWindow.Add("SAS Congruence", JustificationSwitch.DeductionJustType.SAS_CONGRUENCE);
            justWindow.Add("SSS", JustificationSwitch.DeductionJustType.SSS);
            justWindow.Add("Angles of Equal Measureare Congruent", JustificationSwitch.DeductionJustType.ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT);
            justWindow.Add("Transitive Congruent Angle with Right Angle", JustificationSwitch.DeductionJustType.TRANSITIVE_CONGRUENT_ANGLE_WITH_RIGHT_ANGLE);
            //Definitions
            justWindow.Add("Altitude Definition", JustificationSwitch.DeductionJustType.ALTITUDE_DEFINITION);
            justWindow.Add("Angle Bisector Definition", JustificationSwitch.DeductionJustType.ANGLE_BISECTOR_DEFINITION);
            justWindow.Add("Complementary Definition", JustificationSwitch.DeductionJustType.COMPLEMENTARY_DEFINITION);
            justWindow.Add("Congruent Segments => Proportional Segments Defition", JustificationSwitch.DeductionJustType.CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION);
            justWindow.Add("Equilateral Triangle Definition", JustificationSwitch.DeductionJustType.EQUILATERAL_TRIANGLE_DEFINITION);
            justWindow.Add("Isosceles Triangle Definition", JustificationSwitch.DeductionJustType.ISOSCELES_TRIANGLE_DEFINITION);
            justWindow.Add("Median Definition", JustificationSwitch.DeductionJustType.MEDIAN_DEFINITION);
            justWindow.Add("Midpoint Definition", JustificationSwitch.DeductionJustType.MIDPOINT_DEFINITION);
            justWindow.Add("Right Angle Definition", JustificationSwitch.DeductionJustType.RIGHT_ANGLE_DEFINITION);
            justWindow.Add("Right Triangle Definition", JustificationSwitch.DeductionJustType.RIGHT_TRIANGLE_DEFINITION);
            justWindow.Add("Segment Bisector Definition", JustificationSwitch.DeductionJustType.SEGMENT_BISECTOR_DEFINITION);
            justWindow.Add("Supplementary Definition", JustificationSwitch.DeductionJustType.SUPPLEMENTARY_DEFINITION);
            justWindow.Add("Straight Angle Definition", JustificationSwitch.DeductionJustType.STRAIGHT_ANGLE_DEFINITION);
            //Theorems
            justWindow.Add("Perpendicular Definition", JustificationSwitch.DeductionJustType.PERPENDICULAR_DEFINITION);
            justWindow.Add("Perpendicular Bisector Definition", JustificationSwitch.DeductionJustType.PERPENDICULAR_BISECTOR_DEFINITION);
            justWindow.Add("AAS", JustificationSwitch.DeductionJustType.AAS);
            justWindow.Add("Acute Angles in Right Triangle are Complementary", JustificationSwitch.DeductionJustType.ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY);
            justWindow.Add("Congruent Alternate Interior Angles => Parallell", JustificationSwitch.DeductionJustType.ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL);
            justWindow.Add("Altitude of Right Triangles => Similar Triangles", JustificationSwitch.DeductionJustType.ALTITUDE_OF_RIGHT_TRIANGLES_IMPLIES_SIMILAR);
            justWindow.Add("Angle Bisector of Isosceles Triangles is Perpendicular Bisector", JustificationSwitch.DeductionJustType.ANGLE_BISECTOR_IS_PERPENDICULAR_BISECTOR_IN_ISOSCELES);
            justWindow.Add("Angle Bisector Theorem", JustificationSwitch.DeductionJustType.ANGLE_BISECTOR_THEOREM);
            justWindow.Add("Congruent Supplementary Angles Are Right Angles", JustificationSwitch.DeductionJustType.CONGRUENT_SUPPLEMENTARY_ANGLES_IMPLY_RIGHT_ANGLES);
            justWindow.Add("Two Lines Forming Congruent Adjacent => Perpendicular", JustificationSwitch.DeductionJustType.CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR);
            justWindow.Add("Congruent Angles in Triangle => Congruent Opposite Sides", JustificationSwitch.DeductionJustType.CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES);
            justWindow.Add("Congruent Sides in Triangle => Congruent Opposite Angles", JustificationSwitch.DeductionJustType.CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES);
            justWindow.Add("Exterioir Angle Measure Equal to Sum of Remote Interior", JustificationSwitch.DeductionJustType.EXTERIOR_ANGLE_EQUAL_SUM_REMOTE_ANGLES);
            justWindow.Add("Equilateral Triangle Angles Measure 60 Degrees", JustificationSwitch.DeductionJustType.EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES);
            justWindow.Add("Hypotenuse Leg", JustificationSwitch.DeductionJustType.HYPOTENUSE_LEG);
            justWindow.Add("Isosceles Triangle Theorem", JustificationSwitch.DeductionJustType.ISOSCELES_TRIANGLE_THEOREM);
            justWindow.Add("Midpoint Theorem", JustificationSwitch.DeductionJustType.MIDPOINT_THEOREM);
            justWindow.Add("Parallel => Alternate Interioir Angles Congruent", JustificationSwitch.DeductionJustType.PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES);
            justWindow.Add("Parallel => Same Side Interioir Supplementary", JustificationSwitch.DeductionJustType.PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY);
            justWindow.Add("Perpendicular Lines => Congruent Adjacent Angles", JustificationSwitch.DeductionJustType.PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES);
            justWindow.Add("Supplementary (or Complementary) Relations of Congruent Angles are Congruent", JustificationSwitch.DeductionJustType.RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT);
            justWindow.Add("Same Side Supplementary Angles => Parallel", JustificationSwitch.DeductionJustType.SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL);
            justWindow.Add("SAS Similarity", JustificationSwitch.DeductionJustType.SAS_SIMILARITY);
            justWindow.Add("SSS Similarity", JustificationSwitch.DeductionJustType.SSS_SIMILARITY);
            justWindow.Add("Sum of Angles in a Triangle is 180 Degrees", JustificationSwitch.DeductionJustType.SUM_ANGLES_IN_TRIANGLE_180);
            justWindow.Add("Perpendicular Transversal To Parallel Lines => Perpendicular to Both Lines", JustificationSwitch.DeductionJustType.TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR);
            justWindow.Add("Transitivity of Congruent Triangles", JustificationSwitch.DeductionJustType.TRANSITIVE_CONGRUENT_TRIANGLES);
            justWindow.Add("Transitivity of Parallel Lines", JustificationSwitch.DeductionJustType.TRANSITIVE_PARALLEL);
            justWindow.Add("Transitivity of Similar Triangles", JustificationSwitch.DeductionJustType.TRANSITIVE_SIMILAR);
            justWindow.Add("Proportionality of Triangles", JustificationSwitch.DeductionJustType.TRIANGLE_PROPORTIONALITY);
            justWindow.Add("Two pairs of Congruent Angles in Triangle => Third Pair Congruent", JustificationSwitch.DeductionJustType.TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT);
            justWindow.Add("Vertical Angles", JustificationSwitch.DeductionJustType.VERTICAL_ANGLES);
            //Quad Theorems
            justWindow.Add("Opp. Sides in Parallelogram Are Congruent", JustificationSwitch.DeductionJustType.OPPOSITE_SIDES_PARALLELOGRAM_ARE_CONGRUENT);
            justWindow.Add("Opp. Angles in Parallelogram Are Congruent", JustificationSwitch.DeductionJustType.OPPOSITE_ANGLES_PARALLELOGRAM_ARE_CONGRUENT);
            justWindow.Add("Diagonals in Parallelogram Bisect Each Other", JustificationSwitch.DeductionJustType.DIAGONALS_PARALLELOGRAM_BISECT_EACH_OTHER);
            justWindow.Add("Opp. Sides Congruent in Quad => Parallelogram", JustificationSwitch.DeductionJustType.OPPOSITE_SIDES_CONGRUENT_IMPLIES_PARALLELOGRAM);
            justWindow.Add("Opp. Angles Congruent in Quad => Parallelogram", JustificationSwitch.DeductionJustType.OPPOSITE_ANGLES_CONGRUENT_IMPLIES_PARALLELOGRAM);
            justWindow.Add("One Pair Opp. Sides Congruent Parallel => Parallelogram", JustificationSwitch.DeductionJustType.ONE_PAIR_OPPOSITE_SIDES_CONGRUENT_PARALLEL_IMPLIES_PARALLELOGRAM);
            justWindow.Add("Diagonals Bisect Each Other => Parallelogram", JustificationSwitch.DeductionJustType.DIAGONALS_BISECT_EACH_OTHER_IMPLY_PARALLELOGRAM);
            justWindow.Add("Diagonals of a Rectangle are Congruent", JustificationSwitch.DeductionJustType.DIAGONALS_OF_RECTANGLE_ARE_CONGRUENT);
            justWindow.Add("Diagonals of a Kite are Perpendicular", JustificationSwitch.DeductionJustType.DIAGONALS_OF_KITE_ARE_PERPENDICULAR);
            justWindow.Add("Diagonals of a Rhombus are Perpendicular", JustificationSwitch.DeductionJustType.DIAGONALS_OF_RHOMBUS_ARE_PERPENDICULAR);
            justWindow.Add("Diagonals of a Rhmbus Bisect the Angles of a Rhombus", JustificationSwitch.DeductionJustType.DIAGONALS_OF_RHOMBUS_BISECT_ANGLES_OF_RHOMBUS);
            justWindow.Add("Two Congruent Consecutive Sides of a Parallelogram => Rhombus", JustificationSwitch.DeductionJustType.TWO_CONSECUTIVE_SIDES_OF_PARALLELOGRAM_CONGRUENT_IMPLY_RHOMBUS);
            justWindow.Add("Base Angles of Isosceles Trapezoid => Rhombus", JustificationSwitch.DeductionJustType.BASE_ANGLES_OF_ISOSCELES_TRAPEZOID_CONGRUENT);
            justWindow.Add("Median Trapezoid Parallel to Base => Rhombus", JustificationSwitch.DeductionJustType.MEDIAN_TRAPEZOID_PARALLEL_TO_BASE);
            justWindow.Add("Median Trapezoid Length Half Sum Bases => Rhombus", JustificationSwitch.DeductionJustType.MEDIAN_TRAPEZOID_LENGTH_HALF_SUM_BASES);
            //Circles
            justWindow.Add("Definition of Circle: Congruent Radii", JustificationSwitch.DeductionJustType.CIRCLE_DEFINITION);
            justWindow.Add("Circles of Equal Radii Congruent", JustificationSwitch.DeductionJustType.CIRCLE_CONGRUENCE_DEFINITION);
            justWindow.Add("A Tangent Is Perpendicular to a Radius", JustificationSwitch.DeductionJustType.TANGENT_IS_PERPENDICULAR_TO_RADIUS);
            justWindow.Add("Perpendicular to Radius is Tangent", JustificationSwitch.DeductionJustType.PERPENDICULAR_TO_RADIUS_IS_TANGENT);
            justWindow.Add("Tangents to Circle from Same Point Congruent", JustificationSwitch.DeductionJustType.TANGENT_TO_CIRCLE_ARE_CONGRUENT_FROM_SAME_POINT);
            justWindow.Add("Central Angle Congruence => Arc Minor Congruence", JustificationSwitch.DeductionJustType.MINOR_ARCS_CONGRUENT_IF_CENTRAL_ANGLE_CONGRUENT);
            justWindow.Add("Arc Minor Congruence Congruence => Central Angle", JustificationSwitch.DeductionJustType.CENTRAL_ANGLES_CONGRUENT_IF_MINOR_ARCS_CONGRUENT);
            justWindow.Add("Congruent Chords have Congruent Arcs", JustificationSwitch.DeductionJustType.CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS);
            justWindow.Add("Congruent Arcs have Congruent Chords", JustificationSwitch.DeductionJustType.CONGRUENT_ARCS_HAVE_CONGRUENT_CHORDS);
            justWindow.Add("Diameter Perpendicular to Chord Bisects the Chord and Arc", JustificationSwitch.DeductionJustType.DIAMETER_PERPENDICULAR_TO_CHORD_BISECTS_CHORD_AND_ARC);
            justWindow.Add("Measure of Inscribed Angle is Half Intercepted Arc", JustificationSwitch.DeductionJustType.MEASURE_INSCRIBED_ANGLE_EQUAL_HALF_INTERCEPTED_ARC);
            justWindow.Add("Inscribed Angle In Semicircle is Right", JustificationSwitch.DeductionJustType.ANGLE_INSCRIBED_SEMICIRCLE_IS_RIGHT);
            justWindow.Add("Measure Central Angle Equals Measure Intercepted Arc", JustificationSwitch.DeductionJustType.CENTRAL_ANGLE_EQUAL_MEASURE_INTERCEPTED_ARC);
            justWindow.Add("Angle Created by Tangent and Chord is Half Intercepted Arc", JustificationSwitch.DeductionJustType.CHORD_AND_TANGENT_ANGLE_IS_HALF_INTERCEPTED_ARC);
            justWindow.Add("Angle Measure of Exterior Angle is Half Difference of Intercepted Arcs", JustificationSwitch.DeductionJustType.EXTERIOR_ANGLE_HALF_DIFFERENCE_INTERCEPTED_ARCS);
            justWindow.Add("Intersecting Chords Inside a Circle Create Angle Measure Half Sum of Intercepted Arc", JustificationSwitch.DeductionJustType.TWO_INTERSECTING_CHORDS_ANGLE_MEASURE_HALF_SUM_INTERCEPTED_ARCS);
            justWindow.Add("Two Insribed Angles have Equal Measure if they Intercept Same Arc", JustificationSwitch.DeductionJustType.TWO_INTERCEPTED_ARCS_HAVE_CONGRUENT_ANGLES);
            justWindow.Add("Inscribed Quadrilateral has Supplementary Opposite Angles", JustificationSwitch.DeductionJustType.INSCRIBED_QUADRILATERAL_OPPOSITE_ANGLES_SUPPLEMENTARY);

        }

        //private void SelectAll_Checkbox(object sender, EventArgs e)
        //{
        //foreach (var row in grid.Rows)
        //{
        //    var checkBox = (CheckBox)row.FindControl("selectCheckBox");
        // checkBox.Checked = cbSelectAll.Checked;
        //}
        //}

        //public void CheckBoxChanged(Object sender, EventArgs e)
        //{
        //var isSelectAll = true;

        //foreach (var row in grid.Rows)
        //{
        //var checkBox = (CheckBox)row.FindControl("selectCheckBox");

        //if (!checkBox.Checked)
        //{
        //isSelectAll = false;
        //break;
        //}
        //}

        //cbSelectAll.Checked = isSelectAll;
        //}

        //
        // Sarah: TODO: when the "Check Solution box" is clicked by the user this is called.
        //
        //private void OnSolutionCheck(HypergraphWrapper hypergraph)
        //{
        //    List<GroundedClause> statements = new List<GroundedClause>(currentSolution.Values);

        //    // Externally call the function in UI class to check the solution.
        //    int returned = VerifySolution(hypergraph, problem, statements, currentJust);

        //    //
        //    // Handle errors / other situations.
        //    //
        //}

        //
        // Solution verification
        //
        // Returns: 
        //     Fails a one or more steps.
        //     Steps are correct, but solution is incomplete.
        //     Complete Solution
        //public int VerifySolution(HypergraphWrapper hypergraph,
        //                          GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> problem,
        //                          List<GeometryTutorLib.ConcreteAST.GroundedClause> statements,
        //                          List<string> justifications)
        //{
        //    // We need to verify that all statements exist in the hypergraph.
        //    List<int> failedIndices = hypergraph.QueryNodesInGraph(statements);


        //    return 0;
        //}
    }
}
