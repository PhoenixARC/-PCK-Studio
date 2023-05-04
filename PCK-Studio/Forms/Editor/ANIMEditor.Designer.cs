namespace PckStudio.Forms.Editor
{
	partial class ANIMEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ANIMEditor));
            this.saveButton = new MetroFramework.Controls.MetroButton();
            this.effectsGroup = new System.Windows.Forms.GroupBox();
            this.rightLegOCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.headOCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.leftLegOCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.leftArmOCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.bodyOCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.rightLegCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.slimCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.headCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.leftLegCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.rightArmCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.leftArmCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.bodyCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.classicCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.rightArmOCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.effectsGroup2 = new System.Windows.Forms.GroupBox();
            this.rightLeggingCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.helmetCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.leftLeggingCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.rightArmorCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.leftArmorCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.chestplateCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.unknownCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.crouchCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.dinnerboneCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.noArmorCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.bobbingCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.santaCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.syncLegsCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.staticArmsCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.syncArmsCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.statueCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.zombieCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.staticLegsCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.copyButton = new MetroFramework.Controls.MetroButton();
            this.importButton = new MetroFramework.Controls.MetroButton();
            this.exportButton = new MetroFramework.Controls.MetroButton();
            this.animValue = new MetroFramework.Controls.MetroLabel();
            this.uncheckAllButton = new MetroFramework.Controls.MetroButton();
            this.checkAllButton = new MetroFramework.Controls.MetroButton();
            this.toolTip = new MetroFramework.Components.MetroToolTip();
            this.resetButton = new MetroFramework.Controls.MetroButton();
            this.templateButton = new MetroFramework.Controls.MetroButton();
            this.effectsGroup.SuspendLayout();
            this.effectsGroup2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(250, 514);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(126, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.saveButton.UseSelectable = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // effectsGroup
            // 
            this.effectsGroup.Controls.Add(this.rightLegOCheckBox);
            this.effectsGroup.Controls.Add(this.headOCheckBox);
            this.effectsGroup.Controls.Add(this.leftLegOCheckBox);
            this.effectsGroup.Controls.Add(this.leftArmOCheckBox);
            this.effectsGroup.Controls.Add(this.bodyOCheckBox);
            this.effectsGroup.Controls.Add(this.rightLegCheckBox);
            this.effectsGroup.Controls.Add(this.slimCheckBox);
            this.effectsGroup.Controls.Add(this.headCheckBox);
            this.effectsGroup.Controls.Add(this.leftLegCheckBox);
            this.effectsGroup.Controls.Add(this.rightArmCheckBox);
            this.effectsGroup.Controls.Add(this.leftArmCheckBox);
            this.effectsGroup.Controls.Add(this.bodyCheckBox);
            this.effectsGroup.Controls.Add(this.classicCheckBox);
            this.effectsGroup.Controls.Add(this.rightArmOCheckBox);
            this.effectsGroup.ForeColor = System.Drawing.SystemColors.Window;
            this.effectsGroup.Location = new System.Drawing.Point(22, 148);
            this.effectsGroup.Name = "effectsGroup";
            this.effectsGroup.Size = new System.Drawing.Size(393, 238);
            this.effectsGroup.TabIndex = 2;
            this.effectsGroup.TabStop = false;
            this.effectsGroup.Text = "Skin Flags";
            // 
            // rightLegOCheckBox
            // 
            this.rightLegOCheckBox.AutoSize = true;
            this.rightLegOCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rightLegOCheckBox.Location = new System.Drawing.Point(180, 208);
            this.rightLegOCheckBox.Name = "rightLegOCheckBox";
            this.rightLegOCheckBox.Size = new System.Drawing.Size(199, 19);
            this.rightLegOCheckBox.TabIndex = 13;
            this.rightLegOCheckBox.Text = "Remove Right Leg Layer Box";
            this.rightLegOCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.rightLegOCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.rightLegOCheckBox.UseSelectable = true;
            // 
            // headOCheckBox
            // 
            this.headOCheckBox.AutoSize = true;
            this.headOCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.headOCheckBox.Location = new System.Drawing.Point(180, 50);
            this.headOCheckBox.Name = "headOCheckBox";
            this.headOCheckBox.Size = new System.Drawing.Size(173, 19);
            this.headOCheckBox.TabIndex = 12;
            this.headOCheckBox.Text = "Remove Head Layer Box";
            this.headOCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.headOCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.headOCheckBox.UseSelectable = true;
            // 
            // leftLegOCheckBox
            // 
            this.leftLegOCheckBox.AutoSize = true;
            this.leftLegOCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.leftLegOCheckBox.Location = new System.Drawing.Point(180, 174);
            this.leftLegOCheckBox.Name = "leftLegOCheckBox";
            this.leftLegOCheckBox.Size = new System.Drawing.Size(190, 19);
            this.leftLegOCheckBox.TabIndex = 11;
            this.leftLegOCheckBox.Text = "Remove Left Leg Layer Box";
            this.leftLegOCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.leftLegOCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.leftLegOCheckBox.UseSelectable = true;
            // 
            // leftArmOCheckBox
            // 
            this.leftArmOCheckBox.AutoSize = true;
            this.leftArmOCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.leftArmOCheckBox.Location = new System.Drawing.Point(180, 112);
            this.leftArmOCheckBox.Name = "leftArmOCheckBox";
            this.leftArmOCheckBox.Size = new System.Drawing.Size(194, 19);
            this.leftArmOCheckBox.TabIndex = 9;
            this.leftArmOCheckBox.Text = "Remove Left Arm Layer Box";
            this.leftArmOCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.leftArmOCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.leftArmOCheckBox.UseSelectable = true;
            // 
            // bodyOCheckBox
            // 
            this.bodyOCheckBox.AutoSize = true;
            this.bodyOCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.bodyOCheckBox.Location = new System.Drawing.Point(180, 81);
            this.bodyOCheckBox.Name = "bodyOCheckBox";
            this.bodyOCheckBox.Size = new System.Drawing.Size(172, 19);
            this.bodyOCheckBox.TabIndex = 8;
            this.bodyOCheckBox.Text = "Remove Body Layer Box";
            this.bodyOCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.bodyOCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.bodyOCheckBox.UseSelectable = true;
            // 
            // rightLegCheckBox
            // 
            this.rightLegCheckBox.AutoSize = true;
            this.rightLegCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rightLegCheckBox.Location = new System.Drawing.Point(6, 208);
            this.rightLegCheckBox.Name = "rightLegCheckBox";
            this.rightLegCheckBox.Size = new System.Drawing.Size(162, 19);
            this.rightLegCheckBox.TabIndex = 7;
            this.rightLegCheckBox.Text = "Remove Right Leg Box";
            this.rightLegCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.rightLegCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.rightLegCheckBox.UseSelectable = true;
            // 
            // slimCheckBox
            // 
            this.slimCheckBox.AutoSize = true;
            this.slimCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.slimCheckBox.Location = new System.Drawing.Point(180, 19);
            this.slimCheckBox.Name = "slimCheckBox";
            this.slimCheckBox.Size = new System.Drawing.Size(151, 19);
            this.slimCheckBox.TabIndex = 6;
            this.slimCheckBox.Text = "64x64 Alex/Slim Skin";
            this.slimCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.slimCheckBox, "          The 1.8 style skin type with slim arms, overlays for each part, and sep" +
        "arate textures for right and left limbs. Resolution is also set to 64x64.       " +
        "   ");
            this.slimCheckBox.UseSelectable = true;
            // 
            // headCheckBox
            // 
            this.headCheckBox.AutoSize = true;
            this.headCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.headCheckBox.Location = new System.Drawing.Point(6, 50);
            this.headCheckBox.Name = "headCheckBox";
            this.headCheckBox.Size = new System.Drawing.Size(136, 19);
            this.headCheckBox.TabIndex = 5;
            this.headCheckBox.Text = "Remove Head Box";
            this.headCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.headCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.headCheckBox.UseSelectable = true;
            // 
            // leftLegCheckBox
            // 
            this.leftLegCheckBox.AutoSize = true;
            this.leftLegCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.leftLegCheckBox.Location = new System.Drawing.Point(6, 174);
            this.leftLegCheckBox.Name = "leftLegCheckBox";
            this.leftLegCheckBox.Size = new System.Drawing.Size(153, 19);
            this.leftLegCheckBox.TabIndex = 4;
            this.leftLegCheckBox.Text = "Remove Left Leg Box";
            this.leftLegCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.leftLegCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.leftLegCheckBox.UseSelectable = true;
            // 
            // rightArmCheckBox
            // 
            this.rightArmCheckBox.AutoSize = true;
            this.rightArmCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rightArmCheckBox.Location = new System.Drawing.Point(6, 143);
            this.rightArmCheckBox.Name = "rightArmCheckBox";
            this.rightArmCheckBox.Size = new System.Drawing.Size(166, 19);
            this.rightArmCheckBox.TabIndex = 3;
            this.rightArmCheckBox.Text = "Remove Right Arm Box";
            this.rightArmCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.rightArmCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.rightArmCheckBox.UseSelectable = true;
            // 
            // leftArmCheckBox
            // 
            this.leftArmCheckBox.AutoSize = true;
            this.leftArmCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.leftArmCheckBox.Location = new System.Drawing.Point(6, 112);
            this.leftArmCheckBox.Name = "leftArmCheckBox";
            this.leftArmCheckBox.Size = new System.Drawing.Size(157, 19);
            this.leftArmCheckBox.TabIndex = 2;
            this.leftArmCheckBox.Text = "Remove Left Arm Box";
            this.leftArmCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.leftArmCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.leftArmCheckBox.UseSelectable = true;
            // 
            // bodyCheckBox
            // 
            this.bodyCheckBox.AutoSize = true;
            this.bodyCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.bodyCheckBox.Location = new System.Drawing.Point(6, 81);
            this.bodyCheckBox.Name = "bodyCheckBox";
            this.bodyCheckBox.Size = new System.Drawing.Size(135, 19);
            this.bodyCheckBox.TabIndex = 1;
            this.bodyCheckBox.Text = "Remove Body Box";
            this.bodyCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.bodyCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.bodyCheckBox.UseSelectable = true;
            // 
            // classicCheckBox
            // 
            this.classicCheckBox.AutoSize = true;
            this.classicCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.classicCheckBox.Location = new System.Drawing.Point(6, 19);
            this.classicCheckBox.Name = "classicCheckBox";
            this.classicCheckBox.Size = new System.Drawing.Size(136, 19);
            this.classicCheckBox.TabIndex = 0;
            this.classicCheckBox.Text = "64x64 Classic Skin";
            this.classicCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.classicCheckBox, "          The 1.8 style skin type with overlays for each part and separate textur" +
        "es for right and left limbs. Resolution is also set to 64x64.         ");
            this.classicCheckBox.UseSelectable = true;
            // 
            // rightArmOCheckBox
            // 
            this.rightArmOCheckBox.AutoSize = true;
            this.rightArmOCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.rightArmOCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rightArmOCheckBox.Location = new System.Drawing.Point(180, 143);
            this.rightArmOCheckBox.Name = "rightArmOCheckBox";
            this.rightArmOCheckBox.Size = new System.Drawing.Size(203, 19);
            this.rightArmOCheckBox.TabIndex = 10;
            this.rightArmOCheckBox.Text = "Remove Right Arm Layer Box";
            this.rightArmOCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.rightArmOCheckBox, "             Removes the parent Box for this part. You can still make new boxes f" +
        "or this part. Armor will be disabled for this part, but can be rendered again wi" +
        "th the armor flags.              ");
            this.rightArmOCheckBox.UseSelectable = true;
            // 
            // effectsGroup2
            // 
            this.effectsGroup2.Controls.Add(this.rightLeggingCheckBox);
            this.effectsGroup2.Controls.Add(this.helmetCheckBox);
            this.effectsGroup2.Controls.Add(this.leftLeggingCheckBox);
            this.effectsGroup2.Controls.Add(this.rightArmorCheckBox);
            this.effectsGroup2.Controls.Add(this.leftArmorCheckBox);
            this.effectsGroup2.Controls.Add(this.chestplateCheckBox);
            this.effectsGroup2.ForeColor = System.Drawing.SystemColors.Window;
            this.effectsGroup2.Location = new System.Drawing.Point(421, 183);
            this.effectsGroup2.Name = "effectsGroup2";
            this.effectsGroup2.Size = new System.Drawing.Size(188, 203);
            this.effectsGroup2.TabIndex = 14;
            this.effectsGroup2.TabStop = false;
            this.effectsGroup2.Text = "Armor Flags";
            // 
            // rightLeggingCheckBox
            // 
            this.rightLeggingCheckBox.AutoSize = true;
            this.rightLeggingCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rightLeggingCheckBox.Location = new System.Drawing.Point(6, 174);
            this.rightLeggingCheckBox.Name = "rightLeggingCheckBox";
            this.rightLeggingCheckBox.Size = new System.Drawing.Size(173, 19);
            this.rightLeggingCheckBox.TabIndex = 7;
            this.rightLeggingCheckBox.Text = "Render Right Leg Armor";
            this.rightLeggingCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.rightLeggingCheckBox, " Forcefully enables the specified armor piece.");
            this.rightLeggingCheckBox.UseSelectable = true;
            // 
            // helmetCheckBox
            // 
            this.helmetCheckBox.AutoSize = true;
            this.helmetCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.helmetCheckBox.Location = new System.Drawing.Point(6, 19);
            this.helmetCheckBox.Name = "helmetCheckBox";
            this.helmetCheckBox.Size = new System.Drawing.Size(147, 19);
            this.helmetCheckBox.TabIndex = 5;
            this.helmetCheckBox.Text = "Render Head Armor";
            this.helmetCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.helmetCheckBox, " Forcefully enables the specified armor piece.");
            this.helmetCheckBox.UseSelectable = true;
            // 
            // leftLeggingCheckBox
            // 
            this.leftLeggingCheckBox.AutoSize = true;
            this.leftLeggingCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.leftLeggingCheckBox.Location = new System.Drawing.Point(6, 143);
            this.leftLeggingCheckBox.Name = "leftLeggingCheckBox";
            this.leftLeggingCheckBox.Size = new System.Drawing.Size(164, 19);
            this.leftLeggingCheckBox.TabIndex = 4;
            this.leftLeggingCheckBox.Text = "Render Left Leg Armor";
            this.leftLeggingCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.leftLeggingCheckBox, " Forcefully enables the specified armor piece.");
            this.leftLeggingCheckBox.UseSelectable = true;
            // 
            // rightArmorCheckBox
            // 
            this.rightArmorCheckBox.AutoSize = true;
            this.rightArmorCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rightArmorCheckBox.Location = new System.Drawing.Point(6, 112);
            this.rightArmorCheckBox.Name = "rightArmorCheckBox";
            this.rightArmorCheckBox.Size = new System.Drawing.Size(177, 19);
            this.rightArmorCheckBox.TabIndex = 3;
            this.rightArmorCheckBox.Text = "Render Right Arm Armor";
            this.rightArmorCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.rightArmorCheckBox, " Forcefully enables the specified armor piece.");
            this.rightArmorCheckBox.UseSelectable = true;
            // 
            // leftArmorCheckBox
            // 
            this.leftArmorCheckBox.AutoSize = true;
            this.leftArmorCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.leftArmorCheckBox.Location = new System.Drawing.Point(6, 81);
            this.leftArmorCheckBox.Name = "leftArmorCheckBox";
            this.leftArmorCheckBox.Size = new System.Drawing.Size(168, 19);
            this.leftArmorCheckBox.TabIndex = 2;
            this.leftArmorCheckBox.Text = "Render Left Arm Armor";
            this.leftArmorCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.leftArmorCheckBox, " Forcefully enables the specified armor piece.");
            this.leftArmorCheckBox.UseSelectable = true;
            // 
            // chestplateCheckBox
            // 
            this.chestplateCheckBox.AutoSize = true;
            this.chestplateCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.chestplateCheckBox.Location = new System.Drawing.Point(6, 50);
            this.chestplateCheckBox.Name = "chestplateCheckBox";
            this.chestplateCheckBox.Size = new System.Drawing.Size(146, 19);
            this.chestplateCheckBox.TabIndex = 1;
            this.chestplateCheckBox.Text = "Render Body Armor";
            this.chestplateCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.chestplateCheckBox, " Forcefully enables the specified armor piece.");
            this.chestplateCheckBox.UseSelectable = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.unknownCheckBox);
            this.groupBox1.Controls.Add(this.crouchCheckBox);
            this.groupBox1.Controls.Add(this.dinnerboneCheckBox);
            this.groupBox1.Controls.Add(this.noArmorCheckBox);
            this.groupBox1.Controls.Add(this.bobbingCheckBox);
            this.groupBox1.Controls.Add(this.santaCheckBox);
            this.groupBox1.Controls.Add(this.syncLegsCheckBox);
            this.groupBox1.Controls.Add(this.staticArmsCheckBox);
            this.groupBox1.Controls.Add(this.syncArmsCheckBox);
            this.groupBox1.Controls.Add(this.statueCheckBox);
            this.groupBox1.Controls.Add(this.zombieCheckBox);
            this.groupBox1.Controls.Add(this.staticLegsCheckBox);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.groupBox1.Location = new System.Drawing.Point(22, 388);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(587, 115);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Special Animations";
            // 
            // unknownCheckBox
            // 
            this.unknownCheckBox.AutoSize = true;
            this.unknownCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.unknownCheckBox.Location = new System.Drawing.Point(126, 81);
            this.unknownCheckBox.Name = "unknownCheckBox";
            this.unknownCheckBox.Size = new System.Drawing.Size(84, 19);
            this.unknownCheckBox.TabIndex = 13;
            this.unknownCheckBox.Text = "Unknown";
            this.unknownCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.unknownCheckBox, "   If you figure out what this is. Please reach out to MNL#8935 on Discord. (:   " +
        "");
            this.unknownCheckBox.UseSelectable = true;
            // 
            // crouchCheckBox
            // 
            this.crouchCheckBox.AutoSize = true;
            this.crouchCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.crouchCheckBox.Location = new System.Drawing.Point(126, 50);
            this.crouchCheckBox.Name = "crouchCheckBox";
            this.crouchCheckBox.Size = new System.Drawing.Size(137, 19);
            this.crouchCheckBox.TabIndex = 12;
            this.crouchCheckBox.Text = "Backwards Crouch";
            this.crouchCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.crouchCheckBox, "      The crouch animation is reversed so that the arms and body lean back. Usefu" +
        "l for small skins.     ");
            this.crouchCheckBox.UseSelectable = true;
            // 
            // dinnerboneCheckBox
            // 
            this.dinnerboneCheckBox.AutoSize = true;
            this.dinnerboneCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.dinnerboneCheckBox.Location = new System.Drawing.Point(126, 19);
            this.dinnerboneCheckBox.Name = "dinnerboneCheckBox";
            this.dinnerboneCheckBox.Size = new System.Drawing.Size(97, 19);
            this.dinnerboneCheckBox.TabIndex = 11;
            this.dinnerboneCheckBox.Text = "Dinnerbone";
            this.dinnerboneCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dinnerboneCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.dinnerboneCheckBox, "   Flips the skin upside down like Dinnerbone\'s skin, a Minecraft developer.  ");
            this.dinnerboneCheckBox.UseSelectable = true;
            // 
            // noArmorCheckBox
            // 
            this.noArmorCheckBox.AutoSize = true;
            this.noArmorCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.noArmorCheckBox.Location = new System.Drawing.Point(420, 81);
            this.noArmorCheckBox.Name = "noArmorCheckBox";
            this.noArmorCheckBox.Size = new System.Drawing.Size(131, 19);
            this.noArmorCheckBox.TabIndex = 10;
            this.noArmorCheckBox.Text = "Disable All Armor";
            this.noArmorCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.noArmorCheckBox, " Disables all armor desptie the armor flags.  ");
            this.noArmorCheckBox.UseSelectable = true;
            // 
            // bobbingCheckBox
            // 
            this.bobbingCheckBox.AutoSize = true;
            this.bobbingCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.bobbingCheckBox.Location = new System.Drawing.Point(272, 50);
            this.bobbingCheckBox.Name = "bobbingCheckBox";
            this.bobbingCheckBox.Size = new System.Drawing.Size(124, 19);
            this.bobbingCheckBox.TabIndex = 9;
            this.bobbingCheckBox.Text = "Disable Bobbing";
            this.bobbingCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.bobbingCheckBox, " Disables the bobbing effect in first person.");
            this.bobbingCheckBox.UseSelectable = true;
            // 
            // santaCheckBox
            // 
            this.santaCheckBox.AutoSize = true;
            this.santaCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.santaCheckBox.Location = new System.Drawing.Point(420, 50);
            this.santaCheckBox.Name = "santaCheckBox";
            this.santaCheckBox.Size = new System.Drawing.Size(86, 19);
            this.santaCheckBox.TabIndex = 8;
            this.santaCheckBox.Text = "Bad Santa";
            this.santaCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.santaCheckBox, "       The skin sits down after about 10 seconds of no controller input. Made for" +
        " Bad Santa in the \"Festive\" skin pack.       ");
            this.santaCheckBox.UseSelectable = true;
            // 
            // syncLegsCheckBox
            // 
            this.syncLegsCheckBox.AutoSize = true;
            this.syncLegsCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.syncLegsCheckBox.Location = new System.Drawing.Point(272, 19);
            this.syncLegsCheckBox.Name = "syncLegsCheckBox";
            this.syncLegsCheckBox.Size = new System.Drawing.Size(136, 19);
            this.syncLegsCheckBox.TabIndex = 7;
            this.syncLegsCheckBox.Text = "Synchronous Legs";
            this.syncLegsCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.syncLegsCheckBox, "   These parts will move at the same time and angle as each other.  ");
            this.syncLegsCheckBox.UseSelectable = true;
            // 
            // staticArmsCheckBox
            // 
            this.staticArmsCheckBox.AutoSize = true;
            this.staticArmsCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.staticArmsCheckBox.Location = new System.Drawing.Point(6, 19);
            this.staticArmsCheckBox.Name = "staticArmsCheckBox";
            this.staticArmsCheckBox.Size = new System.Drawing.Size(94, 19);
            this.staticArmsCheckBox.TabIndex = 5;
            this.staticArmsCheckBox.Text = "Static Arms";
            this.staticArmsCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.staticArmsCheckBox, " These parts will not move in most animations. ");
            this.staticArmsCheckBox.UseSelectable = true;
            // 
            // syncArmsCheckBox
            // 
            this.syncArmsCheckBox.AutoSize = true;
            this.syncArmsCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.syncArmsCheckBox.Location = new System.Drawing.Point(420, 19);
            this.syncArmsCheckBox.Name = "syncArmsCheckBox";
            this.syncArmsCheckBox.Size = new System.Drawing.Size(140, 19);
            this.syncArmsCheckBox.TabIndex = 4;
            this.syncArmsCheckBox.Text = "Synchronous Arms";
            this.syncArmsCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.syncArmsCheckBox, "   These parts will move at the same time and angle as each other.  ");
            this.syncArmsCheckBox.UseSelectable = true;
            // 
            // statueCheckBox
            // 
            this.statueCheckBox.AutoSize = true;
            this.statueCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.statueCheckBox.Location = new System.Drawing.Point(272, 81);
            this.statueCheckBox.Name = "statueCheckBox";
            this.statueCheckBox.Size = new System.Drawing.Size(126, 19);
            this.statueCheckBox.TabIndex = 3;
            this.statueCheckBox.Text = "Statue of Liberty";
            this.statueCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.statueCheckBox, "       The right arm is lifted like the Statue of Liberty. Made for Angel of Libe" +
        "rty in the \"Doctor Who Volume I\" skin pack.       ");
            this.statueCheckBox.UseSelectable = true;
            // 
            // zombieCheckBox
            // 
            this.zombieCheckBox.AutoSize = true;
            this.zombieCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.zombieCheckBox.Location = new System.Drawing.Point(6, 81);
            this.zombieCheckBox.Name = "zombieCheckBox";
            this.zombieCheckBox.Size = new System.Drawing.Size(107, 19);
            this.zombieCheckBox.TabIndex = 2;
            this.zombieCheckBox.Text = "Zombie Arms";
            this.zombieCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.zombieCheckBox, " Both arms will stick up like a Zombie. ");
            this.zombieCheckBox.UseSelectable = true;
            // 
            // staticLegsCheckBox
            // 
            this.staticLegsCheckBox.AutoSize = true;
            this.staticLegsCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.staticLegsCheckBox.Location = new System.Drawing.Point(6, 50);
            this.staticLegsCheckBox.Name = "staticLegsCheckBox";
            this.staticLegsCheckBox.Size = new System.Drawing.Size(90, 19);
            this.staticLegsCheckBox.TabIndex = 1;
            this.staticLegsCheckBox.Text = "Static Legs";
            this.staticLegsCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTip.SetToolTip(this.staticLegsCheckBox, " These parts will not move in most animations. ");
            this.staticLegsCheckBox.UseSelectable = true;
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(425, 119);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(173, 23);
            this.copyButton.TabIndex = 22;
            this.copyButton.Text = "Copy ANIM Value";
            this.copyButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.copyButton.UseSelectable = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(32, 119);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(186, 23);
            this.importButton.TabIndex = 23;
            this.importButton.Text = "Import ANIM";
            this.importButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.importButton.UseSelectable = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(229, 119);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(186, 23);
            this.exportButton.TabIndex = 24;
            this.exportButton.Text = "Export Template Texture";
            this.exportButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.exportButton.UseSelectable = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // animValue
            // 
            this.animValue.AutoSize = true;
            this.animValue.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.animValue.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.animValue.Location = new System.Drawing.Point(260, 60);
            this.animValue.Name = "animValue";
            this.animValue.Size = new System.Drawing.Size(110, 25);
            this.animValue.TabIndex = 25;
            this.animValue.Text = "0x00000000";
            this.animValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.animValue.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // uncheckAllButton
            // 
            this.uncheckAllButton.Location = new System.Drawing.Point(229, 90);
            this.uncheckAllButton.Name = "uncheckAllButton";
            this.uncheckAllButton.Size = new System.Drawing.Size(186, 23);
            this.uncheckAllButton.TabIndex = 26;
            this.uncheckAllButton.Text = "Uncheck All";
            this.uncheckAllButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.uncheckAllButton.UseSelectable = true;
            this.uncheckAllButton.Click += new System.EventHandler(this.uncheckAllButton_Click);
            // 
            // checkAllButton
            // 
            this.checkAllButton.Location = new System.Drawing.Point(32, 90);
            this.checkAllButton.Name = "checkAllButton";
            this.checkAllButton.Size = new System.Drawing.Size(186, 23);
            this.checkAllButton.TabIndex = 27;
            this.checkAllButton.Text = "Check All";
            this.checkAllButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.checkAllButton.UseSelectable = true;
            this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
            // 
            // toolTip
            // 
            this.toolTip.StripAmpersands = true;
            this.toolTip.Style = MetroFramework.MetroColorStyle.Blue;
            this.toolTip.StyleManager = null;
            this.toolTip.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(425, 90);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(173, 23);
            this.resetButton.TabIndex = 28;
            this.resetButton.Text = "Restore ANIM";
            this.resetButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.resetButton.UseSelectable = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // templateButton
            // 
            this.templateButton.Location = new System.Drawing.Point(425, 154);
            this.templateButton.Name = "templateButton";
            this.templateButton.Size = new System.Drawing.Size(173, 23);
            this.templateButton.TabIndex = 29;
            this.templateButton.Text = "Skin Presets";
            this.templateButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.templateButton.UseSelectable = true;
            this.templateButton.Click += new System.EventHandler(this.templateButton_Click);
            // 
            // ANIMEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(614, 515);
            this.Controls.Add(this.templateButton);
            this.Controls.Add(this.effectsGroup);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.checkAllButton);
            this.Controls.Add(this.uncheckAllButton);
            this.Controls.Add(this.animValue);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.effectsGroup2);
            this.Controls.Add(this.saveButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(630, 554);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(630, 554);
            this.Name = "ANIMEditor";
            this.Text = "ANIM Editor";
            this.effectsGroup.ResumeLayout(false);
            this.effectsGroup.PerformLayout();
            this.effectsGroup2.ResumeLayout(false);
            this.effectsGroup2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private MetroFramework.Controls.MetroButton saveButton;
		private System.Windows.Forms.GroupBox effectsGroup;
		private MetroFramework.Controls.MetroCheckBox headCheckBox;
		private MetroFramework.Controls.MetroCheckBox leftLegCheckBox;
		private MetroFramework.Controls.MetroCheckBox rightArmCheckBox;
		private MetroFramework.Controls.MetroCheckBox leftArmCheckBox;
		private MetroFramework.Controls.MetroCheckBox bodyCheckBox;
		private MetroFramework.Controls.MetroCheckBox classicCheckBox;
		private MetroFramework.Controls.MetroCheckBox slimCheckBox;
		private MetroFramework.Controls.MetroCheckBox rightLegCheckBox;
		private MetroFramework.Controls.MetroCheckBox rightLegOCheckBox;
		private MetroFramework.Controls.MetroCheckBox headOCheckBox;
		private MetroFramework.Controls.MetroCheckBox leftLegOCheckBox;
		private MetroFramework.Controls.MetroCheckBox rightArmOCheckBox;
		private MetroFramework.Controls.MetroCheckBox leftArmOCheckBox;
		private MetroFramework.Controls.MetroCheckBox bodyOCheckBox;
		private System.Windows.Forms.GroupBox effectsGroup2;
		private MetroFramework.Controls.MetroCheckBox rightLeggingCheckBox;
		private MetroFramework.Controls.MetroCheckBox helmetCheckBox;
		private MetroFramework.Controls.MetroCheckBox leftLeggingCheckBox;
		private MetroFramework.Controls.MetroCheckBox rightArmorCheckBox;
		private MetroFramework.Controls.MetroCheckBox leftArmorCheckBox;
		private MetroFramework.Controls.MetroCheckBox chestplateCheckBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private MetroFramework.Controls.MetroCheckBox syncLegsCheckBox;
		private MetroFramework.Controls.MetroCheckBox staticArmsCheckBox;
		private MetroFramework.Controls.MetroCheckBox syncArmsCheckBox;
		private MetroFramework.Controls.MetroCheckBox statueCheckBox;
		private MetroFramework.Controls.MetroCheckBox zombieCheckBox;
		private MetroFramework.Controls.MetroCheckBox staticLegsCheckBox;
		private MetroFramework.Controls.MetroCheckBox bobbingCheckBox;
		private MetroFramework.Controls.MetroCheckBox santaCheckBox;
		private MetroFramework.Controls.MetroCheckBox noArmorCheckBox;
		private MetroFramework.Controls.MetroCheckBox dinnerboneCheckBox;
		private MetroFramework.Controls.MetroCheckBox crouchCheckBox;
		private MetroFramework.Controls.MetroCheckBox unknownCheckBox;
		private MetroFramework.Controls.MetroButton copyButton;
		private MetroFramework.Controls.MetroButton importButton;
		private MetroFramework.Controls.MetroButton exportButton;
		private MetroFramework.Controls.MetroLabel animValue;
		private MetroFramework.Controls.MetroButton uncheckAllButton;
		private MetroFramework.Controls.MetroButton checkAllButton;
		private MetroFramework.Components.MetroToolTip toolTip;
		private MetroFramework.Controls.MetroButton resetButton;
		private MetroFramework.Controls.MetroButton templateButton;
	}
}