﻿namespace Polygons
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panelCanvas = new System.Windows.Forms.Panel();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.addRelation = new System.Windows.Forms.Button();
            this.buttonFixLength = new System.Windows.Forms.Button();
            this.buttonAddVertex = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonMove = new System.Windows.Forms.Button();
            this.buttonDraw = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioButtonLineBresenham = new System.Windows.Forms.RadioButton();
            this.radioButtonLineLibrary = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.panelCanvas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1082, 30);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(133, 26);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 114F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.panelCanvas, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 30);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1082, 723);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // panelCanvas
            // 
            this.panelCanvas.Controls.Add(this.canvas);
            this.panelCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCanvas.Location = new System.Drawing.Point(117, 71);
            this.panelCanvas.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelCanvas.Name = "panelCanvas";
            this.panelCanvas.Size = new System.Drawing.Size(962, 648);
            this.panelCanvas.TabIndex = 0;
            // 
            // canvas
            // 
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(0, 0);
            this.canvas.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(962, 648);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDown);
            this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseMove);
            this.canvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseUp);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.addRelation);
            this.panel1.Controls.Add(this.buttonFixLength);
            this.panel1.Controls.Add(this.buttonAddVertex);
            this.panel1.Controls.Add(this.buttonDelete);
            this.panel1.Controls.Add(this.buttonMove);
            this.panel1.Controls.Add(this.buttonDraw);
            this.panel1.Location = new System.Drawing.Point(117, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(962, 59);
            this.panel1.TabIndex = 2;
            // 
            // addRelation
            // 
            this.addRelation.Location = new System.Drawing.Point(681, 0);
            this.addRelation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.addRelation.Name = "addRelation";
            this.addRelation.Size = new System.Drawing.Size(86, 59);
            this.addRelation.TabIndex = 6;
            this.addRelation.Text = "Add relation";
            this.addRelation.UseVisualStyleBackColor = true;
            this.addRelation.Click += new System.EventHandler(this.addRelation_Click);
            // 
            // buttonFixLength
            // 
            this.buttonFixLength.Location = new System.Drawing.Point(542, 0);
            this.buttonFixLength.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonFixLength.Name = "buttonFixLength";
            this.buttonFixLength.Size = new System.Drawing.Size(86, 59);
            this.buttonFixLength.TabIndex = 5;
            this.buttonFixLength.Text = "Fix length";
            this.buttonFixLength.UseVisualStyleBackColor = true;
            this.buttonFixLength.Click += new System.EventHandler(this.buttonFixLength_Click);
            // 
            // buttonAddVertex
            // 
            this.buttonAddVertex.Location = new System.Drawing.Point(397, 0);
            this.buttonAddVertex.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonAddVertex.Name = "buttonAddVertex";
            this.buttonAddVertex.Size = new System.Drawing.Size(86, 59);
            this.buttonAddVertex.TabIndex = 4;
            this.buttonAddVertex.Text = "Add vertex";
            this.buttonAddVertex.UseVisualStyleBackColor = true;
            this.buttonAddVertex.Click += new System.EventHandler(this.buttonAddVertex_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(270, 0);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(86, 59);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "Delete vertex";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonMove
            // 
            this.buttonMove.Location = new System.Drawing.Point(143, 0);
            this.buttonMove.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(86, 59);
            this.buttonMove.TabIndex = 2;
            this.buttonMove.Text = "Move";
            this.buttonMove.UseVisualStyleBackColor = true;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // buttonDraw
            // 
            this.buttonDraw.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonDraw.Location = new System.Drawing.Point(14, 0);
            this.buttonDraw.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonDraw.Name = "buttonDraw";
            this.buttonDraw.Size = new System.Drawing.Size(86, 59);
            this.buttonDraw.TabIndex = 1;
            this.buttonDraw.Text = "Draw";
            this.buttonDraw.UseVisualStyleBackColor = true;
            this.buttonDraw.Click += new System.EventHandler(this.buttonDraw_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radioButtonLineBresenham);
            this.panel2.Controls.Add(this.radioButtonLineLibrary);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 70);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(108, 650);
            this.panel2.TabIndex = 3;
            // 
            // radioButtonLineBresenham
            // 
            this.radioButtonLineBresenham.AutoSize = true;
            this.radioButtonLineBresenham.Location = new System.Drawing.Point(5, 117);
            this.radioButtonLineBresenham.Name = "radioButtonLineBresenham";
            this.radioButtonLineBresenham.Size = new System.Drawing.Size(103, 24);
            this.radioButtonLineBresenham.TabIndex = 2;
            this.radioButtonLineBresenham.Text = "Bresenham";
            this.radioButtonLineBresenham.UseVisualStyleBackColor = true;
            this.radioButtonLineBresenham.CheckedChanged += new System.EventHandler(this.radioButtonLineBresenham_CheckedChanged);
            // 
            // radioButtonLineLibrary
            // 
            this.radioButtonLineLibrary.AutoSize = true;
            this.radioButtonLineLibrary.Checked = true;
            this.radioButtonLineLibrary.Location = new System.Drawing.Point(5, 87);
            this.radioButtonLineLibrary.Name = "radioButtonLineLibrary";
            this.radioButtonLineLibrary.Size = new System.Drawing.Size(75, 24);
            this.radioButtonLineLibrary.TabIndex = 1;
            this.radioButtonLineLibrary.TabStop = true;
            this.radioButtonLineLibrary.Text = "Library";
            this.radioButtonLineLibrary.UseVisualStyleBackColor = true;
            this.radioButtonLineLibrary.CheckedChanged += new System.EventHandler(this.radioButtonLineLibrary_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "Line drawing\r\nalgorithm";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 753);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Polygons";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.panelCanvas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel;
        private Panel panelCanvas;
        private PictureBox canvas;
        private Button buttonDraw;
        private Panel panel1;
        private Button buttonMove;
        private Panel panel2;
        private RadioButton radioButtonLineBresenham;
        private RadioButton radioButtonLineLibrary;
        private Label label1;
        private Button buttonDelete;
        private Button buttonAddVertex;
        private Button buttonFixLength;
        private Button addRelation;
    }
}