namespace FGame.LocationEditor
{
    partial class EditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolPanel = new System.Windows.Forms.Panel();
            this.tilePanel = new System.Windows.Forms.Panel();
            this.changeTypeButton = new System.Windows.Forms.Button();
            this.tileLabel = new System.Windows.Forms.Label();
            this.objectMovePanel = new System.Windows.Forms.Panel();
            this.moveToButton = new System.Windows.Forms.Button();
            this.layerMinusButton = new System.Windows.Forms.Button();
            this.layerPlusButton = new System.Windows.Forms.Button();
            this.objectInfoLabel = new System.Windows.Forms.Label();
            this.cameraInfoLabel = new System.Windows.Forms.Label();
            this.mevePanel = new System.Windows.Forms.Panel();
            this.moveDownButton = new System.Windows.Forms.Button();
            this.moveUpButton = new System.Windows.Forms.Button();
            this.moveLeftButton = new System.Windows.Forms.Button();
            this.moveRightButton = new System.Windows.Forms.Button();
            this.keyInputTimer = new System.Windows.Forms.Timer(this.components);
            this.gamePoleView = new System.Windows.Forms.PictureBox();
            this.MainMenuStrip.SuspendLayout();
            this.toolPanel.SuspendLayout();
            this.tilePanel.SuspendLayout();
            this.objectMovePanel.SuspendLayout();
            this.mevePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gamePoleView)).BeginInit();
            this.SuspendLayout();
            // 
            // MainMenuStrip
            // 
            this.MainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.MainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip.Name = "MainMenuStrip";
            this.MainMenuStrip.Size = new System.Drawing.Size(917, 24);
            this.MainMenuStrip.TabIndex = 1;
            this.MainMenuStrip.Text = "menuStrip2";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.newToolStripMenuItem.Text = "New...";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileToolStripMenuItem,
            this.chestToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // tileToolStripMenuItem
            // 
            this.tileToolStripMenuItem.Name = "tileToolStripMenuItem";
            this.tileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.tileToolStripMenuItem.Text = "Tile";
            this.tileToolStripMenuItem.Click += new System.EventHandler(this.tileToolStripMenuItem_Click);
            // 
            // chestToolStripMenuItem
            // 
            this.chestToolStripMenuItem.Name = "chestToolStripMenuItem";
            this.chestToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.chestToolStripMenuItem.Text = "Chest";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // toolPanel
            // 
            this.toolPanel.Controls.Add(this.tilePanel);
            this.toolPanel.Controls.Add(this.objectMovePanel);
            this.toolPanel.Controls.Add(this.objectInfoLabel);
            this.toolPanel.Controls.Add(this.cameraInfoLabel);
            this.toolPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolPanel.Location = new System.Drawing.Point(657, 24);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new System.Drawing.Size(260, 566);
            this.toolPanel.TabIndex = 2;
            // 
            // tilePanel
            // 
            this.tilePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tilePanel.Controls.Add(this.changeTypeButton);
            this.tilePanel.Controls.Add(this.tileLabel);
            this.tilePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tilePanel.Location = new System.Drawing.Point(0, 198);
            this.tilePanel.Name = "tilePanel";
            this.tilePanel.Size = new System.Drawing.Size(260, 56);
            this.tilePanel.TabIndex = 6;
            this.tilePanel.Visible = false;
            // 
            // changeTypeButton
            // 
            this.changeTypeButton.Location = new System.Drawing.Point(3, 24);
            this.changeTypeButton.Name = "changeTypeButton";
            this.changeTypeButton.Size = new System.Drawing.Size(126, 23);
            this.changeTypeButton.TabIndex = 1;
            this.changeTypeButton.Text = "Change Type...";
            this.changeTypeButton.UseVisualStyleBackColor = true;
            this.changeTypeButton.Click += new System.EventHandler(this.changeTypeButton_Click);
            // 
            // tileLabel
            // 
            this.tileLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tileLabel.Location = new System.Drawing.Point(0, 0);
            this.tileLabel.Name = "tileLabel";
            this.tileLabel.Size = new System.Drawing.Size(258, 21);
            this.tileLabel.TabIndex = 0;
            // 
            // objectMovePanel
            // 
            this.objectMovePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objectMovePanel.Controls.Add(this.moveToButton);
            this.objectMovePanel.Controls.Add(this.layerMinusButton);
            this.objectMovePanel.Controls.Add(this.layerPlusButton);
            this.objectMovePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.objectMovePanel.Location = new System.Drawing.Point(0, 136);
            this.objectMovePanel.Name = "objectMovePanel";
            this.objectMovePanel.Size = new System.Drawing.Size(260, 62);
            this.objectMovePanel.TabIndex = 4;
            this.objectMovePanel.Visible = false;
            // 
            // moveToButton
            // 
            this.moveToButton.Location = new System.Drawing.Point(5, 32);
            this.moveToButton.Name = "moveToButton";
            this.moveToButton.Size = new System.Drawing.Size(124, 23);
            this.moveToButton.TabIndex = 7;
            this.moveToButton.TabStop = false;
            this.moveToButton.Text = "Move To...";
            this.moveToButton.UseVisualStyleBackColor = true;
            this.moveToButton.Click += new System.EventHandler(this.moveToButton_Click);
            // 
            // layerMinusButton
            // 
            this.layerMinusButton.Location = new System.Drawing.Point(135, 3);
            this.layerMinusButton.Name = "layerMinusButton";
            this.layerMinusButton.Size = new System.Drawing.Size(119, 23);
            this.layerMinusButton.TabIndex = 5;
            this.layerMinusButton.TabStop = false;
            this.layerMinusButton.Text = "Layer -";
            this.layerMinusButton.UseVisualStyleBackColor = true;
            this.layerMinusButton.Click += new System.EventHandler(this.layerMinusButton_Click);
            // 
            // layerPlusButton
            // 
            this.layerPlusButton.Location = new System.Drawing.Point(3, 3);
            this.layerPlusButton.Name = "layerPlusButton";
            this.layerPlusButton.Size = new System.Drawing.Size(126, 23);
            this.layerPlusButton.TabIndex = 4;
            this.layerPlusButton.TabStop = false;
            this.layerPlusButton.Text = "Layer +";
            this.layerPlusButton.UseVisualStyleBackColor = true;
            this.layerPlusButton.Click += new System.EventHandler(this.layerPlusButton_Click);
            // 
            // objectInfoLabel
            // 
            this.objectInfoLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objectInfoLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.objectInfoLabel.Location = new System.Drawing.Point(0, 46);
            this.objectInfoLabel.Name = "objectInfoLabel";
            this.objectInfoLabel.Size = new System.Drawing.Size(260, 90);
            this.objectInfoLabel.TabIndex = 0;
            // 
            // cameraInfoLabel
            // 
            this.cameraInfoLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cameraInfoLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.cameraInfoLabel.Location = new System.Drawing.Point(0, 0);
            this.cameraInfoLabel.Name = "cameraInfoLabel";
            this.cameraInfoLabel.Size = new System.Drawing.Size(260, 46);
            this.cameraInfoLabel.TabIndex = 1;
            // 
            // mevePanel
            // 
            this.mevePanel.Controls.Add(this.moveDownButton);
            this.mevePanel.Controls.Add(this.moveUpButton);
            this.mevePanel.Controls.Add(this.moveLeftButton);
            this.mevePanel.Controls.Add(this.moveRightButton);
            this.mevePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mevePanel.Location = new System.Drawing.Point(0, 518);
            this.mevePanel.Name = "mevePanel";
            this.mevePanel.Size = new System.Drawing.Size(657, 72);
            this.mevePanel.TabIndex = 4;
            // 
            // moveDownButton
            // 
            this.moveDownButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.moveDownButton.Location = new System.Drawing.Point(185, 44);
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(284, 25);
            this.moveDownButton.TabIndex = 3;
            this.moveDownButton.TabStop = false;
            this.moveDownButton.Text = "\\/ \\/";
            this.moveDownButton.UseVisualStyleBackColor = true;
            this.moveDownButton.Click += new System.EventHandler(this.moveDownButton_Click);
            // 
            // moveUpButton
            // 
            this.moveUpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.moveUpButton.Location = new System.Drawing.Point(185, 6);
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(284, 32);
            this.moveUpButton.TabIndex = 2;
            this.moveUpButton.TabStop = false;
            this.moveUpButton.Text = "/\\ /\\";
            this.moveUpButton.UseVisualStyleBackColor = true;
            this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // moveLeftButton
            // 
            this.moveLeftButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.moveLeftButton.Location = new System.Drawing.Point(3, 6);
            this.moveLeftButton.Name = "moveLeftButton";
            this.moveLeftButton.Size = new System.Drawing.Size(176, 63);
            this.moveLeftButton.TabIndex = 1;
            this.moveLeftButton.TabStop = false;
            this.moveLeftButton.Text = "<<";
            this.moveLeftButton.UseVisualStyleBackColor = true;
            this.moveLeftButton.Click += new System.EventHandler(this.moveLeftButton_Click);
            // 
            // moveRightButton
            // 
            this.moveRightButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.moveRightButton.Location = new System.Drawing.Point(475, 6);
            this.moveRightButton.Name = "moveRightButton";
            this.moveRightButton.Size = new System.Drawing.Size(176, 63);
            this.moveRightButton.TabIndex = 0;
            this.moveRightButton.TabStop = false;
            this.moveRightButton.Text = ">>";
            this.moveRightButton.UseVisualStyleBackColor = true;
            this.moveRightButton.Click += new System.EventHandler(this.moveRightButton_Click);
            // 
            // keyInputTimer
            // 
            this.keyInputTimer.Enabled = true;
            this.keyInputTimer.Interval = 300;
            this.keyInputTimer.Tick += new System.EventHandler(this.keyInputTimer_Tick);
            // 
            // gamePoleView
            // 
            this.gamePoleView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gamePoleView.Location = new System.Drawing.Point(0, 24);
            this.gamePoleView.Name = "gamePoleView";
            this.gamePoleView.Size = new System.Drawing.Size(657, 494);
            this.gamePoleView.TabIndex = 3;
            this.gamePoleView.TabStop = false;
            this.gamePoleView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gamePoleView_MouseClick);
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 590);
            this.Controls.Add(this.gamePoleView);
            this.Controls.Add(this.mevePanel);
            this.Controls.Add(this.toolPanel);
            this.Controls.Add(this.MainMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "EditorForm";
            this.Text = "FGame Location Editor";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.EditorForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditorForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EditorForm_KeyUp);
            this.MainMenuStrip.ResumeLayout(false);
            this.MainMenuStrip.PerformLayout();
            this.toolPanel.ResumeLayout(false);
            this.tilePanel.ResumeLayout(false);
            this.objectMovePanel.ResumeLayout(false);
            this.mevePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gamePoleView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Panel toolPanel;
        private System.Windows.Forms.PictureBox gamePoleView;
        private System.Windows.Forms.Panel mevePanel;
        private System.Windows.Forms.Button moveDownButton;
        private System.Windows.Forms.Button moveUpButton;
        private System.Windows.Forms.Button moveLeftButton;
        private System.Windows.Forms.Button moveRightButton;
        private System.Windows.Forms.Label objectInfoLabel;
        private System.Windows.Forms.Timer keyInputTimer;
        private System.Windows.Forms.Label cameraInfoLabel;
        private System.Windows.Forms.Panel objectMovePanel;
        private System.Windows.Forms.Button moveToButton;
        private System.Windows.Forms.Button layerMinusButton;
        private System.Windows.Forms.Button layerPlusButton;
        private System.Windows.Forms.Panel tilePanel;
        private System.Windows.Forms.Button changeTypeButton;
        private System.Windows.Forms.Label tileLabel;
    }
}

