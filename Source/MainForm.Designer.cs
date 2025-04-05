using SkiaSharp.Views.Desktop;

namespace squad_dma
{
    partial class MainForm
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
            if (disposing && (components is not null))
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
            components = new System.ComponentModel.Container();
            colDialog = new ColorDialog();
            toolTip = new ToolTip(components);
            chkShowEnemyDistance = new CheckBox();
            btnDumpNames = new Button();
            trkUIScale = new TrackBar();
            trkAimLength = new TrackBar();
            chkShowMapSetup = new CheckBox();
            btnToggleMap = new Button();
            btnRestartRadar = new Button();
            txtMapSetupX = new TextBox();
            txtMapSetupY = new TextBox();
            txtMapSetupScale = new TextBox();
            chkMapFree = new CheckBox();
            btnApplyMapScale = new Button();
            tabSettings = new TabPage();
            grpConfig = new GroupBox();
            grpUserInterface = new GroupBox();
            lblAimline = new Label();
            lblUIScale = new Label();
            grpRadar = new GroupBox();
            grpLocalSoldier = new GroupBox();
            chkDisableSuppression = new CheckBox();
            chkSetInteractionDistances = new CheckBox();
            chkAllowShootingInMainBase = new CheckBox();
            chkSpeedHack = new CheckBox();
            chkAirStuck = new CheckBox();
            chkHideActor = new CheckBox();
            grpKeybinds = new GroupBox();
            lblKeybindSetInteractionDistances = new Label();
            btnKeybindSetInteractionDistances = new Button();
            lblKeybindAllowShootingInMainBase = new Label();
            btnKeybindAllowShootingInMainBase = new Button();
            lblKeybindSpeedHack = new Label();
            btnKeybindSpeedHack = new Button();
            lblKeybindAirStuck = new Label();
            btnKeybindAirStuck = new Button();
            lblKeybindHideActor = new Label();
            btnKeybindHideActor = new Button();
            lblKeybindToggleEnemyDistance = new Label();
            btnKeybindToggleEnemyDistance = new Button();
            lblKeybindToggleMap = new Label();
            btnKeybindToggleMap = new Button();
            lblKeybindToggleFullscreen = new Label();
            btnKeybindToggleFullscreen = new Button();
            lblStatusSetInteractionDistances = new Label();
            lblStatusAllowShootingInMainBase = new Label();
            lblStatusSpeedHack = new Label();
            lblStatusAirStuck = new Label();
            lblStatusHideActor = new Label();
            lblStatusToggleEnemyDistance = new Label();
            lblKeybindDumpNames = new Label();
            btnKeybindDumpNames = new Button();
            lblKeybindZoomIn = new Label();
            btnKeybindZoomIn = new Button();
            lblKeybindZoomOut = new Label();
            btnKeybindZoomOut = new Button();
            tabRadar = new TabPage();
            ticketsPanel = new Panel();
            grpMapSetup = new GroupBox();
            lblMapScale = new Label();
            lblMapXY = new Label();
            lblMapCoords = new Label();
            tabControl = new TabControl();
            ((System.ComponentModel.ISupportInitialize)trkUIScale).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkAimLength).BeginInit();
            tabSettings.SuspendLayout();
            grpConfig.SuspendLayout();
            grpUserInterface.SuspendLayout();
            grpRadar.SuspendLayout();
            grpLocalSoldier.SuspendLayout();
            grpKeybinds.SuspendLayout();
            tabRadar.SuspendLayout();
            grpMapSetup.SuspendLayout();
            tabControl.SuspendLayout();
            SuspendLayout();
            // 
            // colDialog
            // 
            colDialog.FullOpen = true;
            // 
            // chkShowEnemyDistance
            // 
            chkShowEnemyDistance.AutoSize = true;
            chkShowEnemyDistance.Font = new Font("Segoe UI", 9F);
            chkShowEnemyDistance.Location = new Point(265, 86);
            chkShowEnemyDistance.Name = "chkShowEnemyDistance";
            chkShowEnemyDistance.Size = new Size(165, 19);
            chkShowEnemyDistance.TabIndex = 15;
            chkShowEnemyDistance.Text = "Show Enemy Distance (F4)";
            toolTip.SetToolTip(chkShowEnemyDistance, "Toggle display of enemy distance on the radar");
            chkShowEnemyDistance.UseVisualStyleBackColor = true;
            // 
            // btnDumpNames
            // 
            btnDumpNames.Font = new Font("Segoe UI", 9F);
            btnDumpNames.Location = new Point(265, 50);
            btnDumpNames.Name = "btnDumpNames";
            btnDumpNames.Size = new Size(200, 30);
            btnDumpNames.TabIndex = 16;
            btnDumpNames.Text = "Dump Names (F6)";
            toolTip.SetToolTip(btnDumpNames, "Dump entity names in the game instance");
            btnDumpNames.UseVisualStyleBackColor = true;
            btnDumpNames.Click += btnDumpNames_Click;
            // 
            // trkUIScale
            // 
            trkUIScale.LargeChange = 10;
            trkUIScale.Location = new Point(15, 50);
            trkUIScale.Maximum = 200;
            trkUIScale.Minimum = 50;
            trkUIScale.Name = "trkUIScale";
            trkUIScale.Size = new Size(200, 45);
            trkUIScale.SmallChange = 5;
            trkUIScale.TabIndex = 14;
            trkUIScale.TickFrequency = 10;
            trkUIScale.TickStyle = TickStyle.None;
            toolTip.SetToolTip(trkUIScale, "Adjust the overall UI scale");
            trkUIScale.Value = 100;
            trkUIScale.Scroll += trkUIScale_Scroll;
            // 
            // trkAimLength
            // 
            trkAimLength.LargeChange = 50;
            trkAimLength.Location = new Point(15, 125);
            trkAimLength.Margin = new Padding(4, 3, 4, 3);
            trkAimLength.Maximum = 2000;
            trkAimLength.Minimum = 10;
            trkAimLength.Name = "trkAimLength";
            trkAimLength.Size = new Size(200, 45);
            trkAimLength.SmallChange = 5;
            trkAimLength.TabIndex = 11;
            trkAimLength.TickFrequency = 50;
            trkAimLength.TickStyle = TickStyle.None;
            toolTip.SetToolTip(trkAimLength, "Adjust the length of the aimline");
            trkAimLength.Value = 500;
            trkAimLength.Scroll += trkAimLength_Scroll;
            // 
            // chkShowMapSetup
            // 
            chkShowMapSetup.AutoSize = true;
            chkShowMapSetup.Font = new Font("Segoe UI", 9F);
            chkShowMapSetup.Location = new Point(15, 30);
            chkShowMapSetup.Name = "chkShowMapSetup";
            chkShowMapSetup.Size = new Size(115, 19);
            chkShowMapSetup.TabIndex = 1;
            chkShowMapSetup.Text = "Show Map Setup";
            toolTip.SetToolTip(chkShowMapSetup, "Show/hide the map setup panel");
            chkShowMapSetup.UseVisualStyleBackColor = true;
            chkShowMapSetup.CheckedChanged += chkShowMapSetup_CheckedChanged;
            // 
            // btnToggleMap
            // 
            btnToggleMap.Font = new Font("Segoe UI", 9F);
            btnToggleMap.Location = new Point(15, 60);
            btnToggleMap.Name = "btnToggleMap";
            btnToggleMap.Size = new Size(200, 30);
            btnToggleMap.TabIndex = 2;
            btnToggleMap.Text = "Toggle Map (F5)";
            toolTip.SetToolTip(btnToggleMap, "Switch between available maps");
            btnToggleMap.UseVisualStyleBackColor = true;
            btnToggleMap.Click += btnToggleMap_Click;
            // 
            // btnRestartRadar
            // 
            btnRestartRadar.Font = new Font("Segoe UI", 9F);
            btnRestartRadar.Location = new Point(230, 60);
            btnRestartRadar.Name = "btnRestartRadar";
            btnRestartRadar.Size = new Size(200, 30);
            btnRestartRadar.TabIndex = 0;
            btnRestartRadar.Text = "Restart Radar";
            toolTip.SetToolTip(btnRestartRadar, "Restart the radar functionality");
            btnRestartRadar.UseVisualStyleBackColor = true;
            btnRestartRadar.Click += btnRestartRadar_Click;
            // 
            // txtMapSetupX
            // 
            txtMapSetupX.Font = new Font("Segoe UI", 9F);
            txtMapSetupX.Location = new Point(70, 57);
            txtMapSetupX.Name = "txtMapSetupX";
            txtMapSetupX.Size = new Size(100, 23);
            txtMapSetupX.TabIndex = 11;
            toolTip.SetToolTip(txtMapSetupX, "X coordinate for map positioning");
            // 
            // txtMapSetupY
            // 
            txtMapSetupY.Font = new Font("Segoe UI", 9F);
            txtMapSetupY.Location = new Point(180, 57);
            txtMapSetupY.Name = "txtMapSetupY";
            txtMapSetupY.Size = new Size(100, 23);
            txtMapSetupY.TabIndex = 13;
            toolTip.SetToolTip(txtMapSetupY, "Y coordinate for map positioning");
            // 
            // txtMapSetupScale
            // 
            txtMapSetupScale.Font = new Font("Segoe UI", 9F);
            txtMapSetupScale.Location = new Point(70, 87);
            txtMapSetupScale.Name = "txtMapSetupScale";
            txtMapSetupScale.Size = new Size(100, 23);
            txtMapSetupScale.TabIndex = 15;
            toolTip.SetToolTip(txtMapSetupScale, "Map scale factor");
            // 
            // chkMapFree
            // 
            chkMapFree.Appearance = Appearance.Button;
            chkMapFree.AutoSize = true;
            chkMapFree.Font = new Font("Segoe UI", 9F);
            chkMapFree.Location = new Point(3, 3);
            chkMapFree.Name = "chkMapFree";
            chkMapFree.Size = new Size(79, 25);
            chkMapFree.TabIndex = 17;
            chkMapFree.Text = "Map Follow";
            chkMapFree.TextAlign = ContentAlignment.MiddleCenter;
            toolTip.SetToolTip(chkMapFree, "Toggle between map following player and free movement");
            chkMapFree.UseVisualStyleBackColor = true;
            chkMapFree.CheckedChanged += chkMapFree_CheckedChanged;
            // 
            // btnApplyMapScale
            // 
            btnApplyMapScale.Font = new Font("Segoe UI", 9F);
            btnApplyMapScale.Location = new Point(180, 120);
            btnApplyMapScale.Name = "btnApplyMapScale";
            btnApplyMapScale.Size = new Size(100, 30);
            btnApplyMapScale.TabIndex = 18;
            btnApplyMapScale.Text = "Apply";
            toolTip.SetToolTip(btnApplyMapScale, "Apply the map setup changes");
            btnApplyMapScale.UseVisualStyleBackColor = true;
            btnApplyMapScale.Click += btnApplyMapScale_Click;
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(grpConfig);
            tabSettings.Location = new Point(4, 24);
            tabSettings.Name = "tabSettings";
            tabSettings.Padding = new Padding(3);
            tabSettings.Size = new Size(1592, 872);
            tabSettings.TabIndex = 1;
            tabSettings.Text = "Settings";
            tabSettings.UseVisualStyleBackColor = true;
            // 
            // grpConfig
            // 
            grpConfig.Controls.Add(grpUserInterface);
            grpConfig.Controls.Add(grpRadar);
            grpConfig.Controls.Add(grpLocalSoldier);
            grpConfig.Controls.Add(grpKeybinds);
            grpConfig.Dock = DockStyle.Fill;
            grpConfig.Location = new Point(3, 3);
            grpConfig.Margin = new Padding(15);
            grpConfig.Name = "grpConfig";
            grpConfig.Padding = new Padding(15);
            grpConfig.Size = new Size(1586, 866);
            grpConfig.TabIndex = 8;
            grpConfig.TabStop = false;
            grpConfig.Text = "Radar Config";
            // 
            // grpUserInterface
            // 
            grpUserInterface.Controls.Add(trkAimLength);
            grpUserInterface.Controls.Add(lblAimline);
            grpUserInterface.Controls.Add(lblUIScale);
            grpUserInterface.Controls.Add(trkUIScale);
            grpUserInterface.Controls.Add(chkShowEnemyDistance);
            grpUserInterface.Controls.Add(btnDumpNames);
            grpUserInterface.Location = new Point(15, 24);
            grpUserInterface.Name = "grpUserInterface";
            grpUserInterface.Padding = new Padding(15);
            grpUserInterface.Size = new Size(500, 250);
            grpUserInterface.TabIndex = 26;
            grpUserInterface.TabStop = false;
            grpUserInterface.Text = "User Interface";
            // 
            // lblAimline
            // 
            lblAimline.AutoSize = true;
            lblAimline.Font = new Font("Segoe UI", 9F);
            lblAimline.Location = new Point(15, 107);
            lblAimline.Name = "lblAimline";
            lblAimline.Size = new Size(88, 15);
            lblAimline.TabIndex = 12;
            lblAimline.Text = "Aimline Length";
            lblAimline.Click += lblAimline_Click;
            // 
            // lblUIScale
            // 
            lblUIScale.AutoSize = true;
            lblUIScale.Font = new Font("Segoe UI", 9F);
            lblUIScale.Location = new Point(15, 30);
            lblUIScale.Name = "lblUIScale";
            lblUIScale.Size = new Size(48, 15);
            lblUIScale.TabIndex = 13;
            lblUIScale.Text = "UI Scale";
            // 
            // grpRadar
            // 
            grpRadar.Controls.Add(btnRestartRadar);
            grpRadar.Controls.Add(chkShowMapSetup);
            grpRadar.Controls.Add(btnToggleMap);
            grpRadar.Location = new Point(15, 280);
            grpRadar.Name = "grpRadar";
            grpRadar.Padding = new Padding(15);
            grpRadar.Size = new Size(500, 120);
            grpRadar.TabIndex = 26;
            grpRadar.TabStop = false;
            grpRadar.Text = "Radar";
            // 
            // grpLocalSoldier
            // 
            grpLocalSoldier.Controls.Add(chkDisableSuppression);
            grpLocalSoldier.Controls.Add(chkSetInteractionDistances);
            grpLocalSoldier.Controls.Add(chkAllowShootingInMainBase);
            grpLocalSoldier.Controls.Add(chkSpeedHack);
            grpLocalSoldier.Controls.Add(chkAirStuck);
            grpLocalSoldier.Controls.Add(chkHideActor);
            grpLocalSoldier.Location = new Point(15, 406);
            grpLocalSoldier.Name = "grpLocalSoldier";
            grpLocalSoldier.Size = new Size(500, 174);
            grpLocalSoldier.TabIndex = 27;
            grpLocalSoldier.TabStop = false;
            grpLocalSoldier.Text = "Local Soldier Features";
            // 
            // chkDisableSuppression
            // 
            chkDisableSuppression.AutoSize = true;
            chkDisableSuppression.Location = new Point(10, 20);
            chkDisableSuppression.Name = "chkDisableSuppression";
            chkDisableSuppression.Size = new Size(131, 19);
            chkDisableSuppression.TabIndex = 0;
            chkDisableSuppression.Text = "Disable Suppression";
            chkDisableSuppression.UseVisualStyleBackColor = true;
            // 
            // chkSetInteractionDistances
            // 
            chkSetInteractionDistances.AutoSize = true;
            chkSetInteractionDistances.Location = new Point(10, 45);
            chkSetInteractionDistances.Name = "chkSetInteractionDistances";
            chkSetInteractionDistances.Size = new Size(177, 19);
            chkSetInteractionDistances.TabIndex = 1;
            chkSetInteractionDistances.Text = "Increase Interaction Distance";
            chkSetInteractionDistances.UseVisualStyleBackColor = true;
            // 
            // chkAllowShootingInMainBase
            // 
            chkAllowShootingInMainBase.AutoSize = true;
            chkAllowShootingInMainBase.Location = new Point(10, 70);
            chkAllowShootingInMainBase.Name = "chkAllowShootingInMainBase";
            chkAllowShootingInMainBase.Size = new Size(177, 19);
            chkAllowShootingInMainBase.TabIndex = 2;
            chkAllowShootingInMainBase.Text = "Allow Shooting in Main Base";
            chkAllowShootingInMainBase.UseVisualStyleBackColor = true;
            // 
            // chkSpeedHack
            // 
            chkSpeedHack.AutoSize = true;
            chkSpeedHack.Location = new Point(10, 95);
            chkSpeedHack.Name = "chkSpeedHack";
            chkSpeedHack.Size = new Size(88, 19);
            chkSpeedHack.TabIndex = 3;
            chkSpeedHack.Text = "Speed Hack";
            chkSpeedHack.UseVisualStyleBackColor = true;
            // 
            // chkAirStuck
            // 
            chkAirStuck.AutoSize = true;
            chkAirStuck.Location = new Point(10, 120);
            chkAirStuck.Name = "chkAirStuck";
            chkAirStuck.Size = new Size(73, 19);
            chkAirStuck.TabIndex = 4;
            chkAirStuck.Text = "Air Stuck";
            chkAirStuck.UseVisualStyleBackColor = true;
            // 
            // chkHideActor
            // 
            chkHideActor.AutoSize = true;
            chkHideActor.Location = new Point(10, 145);
            chkHideActor.Name = "chkHideActor";
            chkHideActor.Size = new Size(83, 19);
            chkHideActor.TabIndex = 5;
            chkHideActor.Text = "Hide Actor";
            chkHideActor.UseVisualStyleBackColor = true;
            // 
            // grpKeybinds
            // 
            grpKeybinds.Controls.Add(lblKeybindSetInteractionDistances);
            grpKeybinds.Controls.Add(btnKeybindSetInteractionDistances);
            grpKeybinds.Controls.Add(lblKeybindAllowShootingInMainBase);
            grpKeybinds.Controls.Add(btnKeybindAllowShootingInMainBase);
            grpKeybinds.Controls.Add(lblKeybindSpeedHack);
            grpKeybinds.Controls.Add(btnKeybindSpeedHack);
            grpKeybinds.Controls.Add(lblKeybindAirStuck);
            grpKeybinds.Controls.Add(btnKeybindAirStuck);
            grpKeybinds.Controls.Add(lblKeybindHideActor);
            grpKeybinds.Controls.Add(btnKeybindHideActor);
            grpKeybinds.Controls.Add(lblKeybindToggleEnemyDistance);
            grpKeybinds.Controls.Add(btnKeybindToggleEnemyDistance);
            grpKeybinds.Controls.Add(lblKeybindToggleMap);
            grpKeybinds.Controls.Add(btnKeybindToggleMap);
            grpKeybinds.Controls.Add(lblKeybindToggleFullscreen);
            grpKeybinds.Controls.Add(btnKeybindToggleFullscreen);
            grpKeybinds.Controls.Add(lblStatusSetInteractionDistances);
            grpKeybinds.Controls.Add(lblStatusAllowShootingInMainBase);
            grpKeybinds.Controls.Add(lblStatusSpeedHack);
            grpKeybinds.Controls.Add(lblStatusAirStuck);
            grpKeybinds.Controls.Add(lblStatusHideActor);
            grpKeybinds.Controls.Add(lblStatusToggleEnemyDistance);
            grpKeybinds.Controls.Add(lblKeybindDumpNames);
            grpKeybinds.Controls.Add(btnKeybindDumpNames);
            grpKeybinds.Controls.Add(lblKeybindZoomIn);
            grpKeybinds.Controls.Add(btnKeybindZoomIn);
            grpKeybinds.Controls.Add(lblKeybindZoomOut);
            grpKeybinds.Controls.Add(btnKeybindZoomOut);
            grpKeybinds.Location = new Point(530, 24);
            grpKeybinds.Name = "grpKeybinds";
            grpKeybinds.Padding = new Padding(15);
            grpKeybinds.Size = new Size(528, 383);
            grpKeybinds.TabIndex = 28;
            grpKeybinds.TabStop = false;
            grpKeybinds.Text = "Keybinds";
            // 
            // lblKeybindSetInteractionDistances
            // 
            lblKeybindSetInteractionDistances.Location = new Point(15, 50);
            lblKeybindSetInteractionDistances.Name = "lblKeybindSetInteractionDistances";
            lblKeybindSetInteractionDistances.Size = new Size(200, 20);
            lblKeybindSetInteractionDistances.TabIndex = 2;
            lblKeybindSetInteractionDistances.Text = "Set Interaction Distances";
            // 
            // btnKeybindSetInteractionDistances
            // 
            btnKeybindSetInteractionDistances.Location = new Point(220, 50);
            btnKeybindSetInteractionDistances.Name = "btnKeybindSetInteractionDistances";
            btnKeybindSetInteractionDistances.Size = new Size(100, 20);
            btnKeybindSetInteractionDistances.TabIndex = 3;
            btnKeybindSetInteractionDistances.Text = "None";
            btnKeybindSetInteractionDistances.Click += BtnKeybindSetInteractionDistances_Click;
            // 
            // lblKeybindAllowShootingInMainBase
            // 
            lblKeybindAllowShootingInMainBase.Location = new Point(15, 80);
            lblKeybindAllowShootingInMainBase.Name = "lblKeybindAllowShootingInMainBase";
            lblKeybindAllowShootingInMainBase.Size = new Size(200, 20);
            lblKeybindAllowShootingInMainBase.TabIndex = 4;
            lblKeybindAllowShootingInMainBase.Text = "Allow Shooting in Main Base";
            // 
            // btnKeybindAllowShootingInMainBase
            // 
            btnKeybindAllowShootingInMainBase.Location = new Point(220, 80);
            btnKeybindAllowShootingInMainBase.Name = "btnKeybindAllowShootingInMainBase";
            btnKeybindAllowShootingInMainBase.Size = new Size(100, 20);
            btnKeybindAllowShootingInMainBase.TabIndex = 5;
            btnKeybindAllowShootingInMainBase.Text = "None";
            btnKeybindAllowShootingInMainBase.Click += BtnKeybindAllowShootingInMainBase_Click;
            // 
            // lblKeybindSpeedHack
            // 
            lblKeybindSpeedHack.Location = new Point(15, 110);
            lblKeybindSpeedHack.Name = "lblKeybindSpeedHack";
            lblKeybindSpeedHack.Size = new Size(200, 20);
            lblKeybindSpeedHack.TabIndex = 6;
            lblKeybindSpeedHack.Text = "Speed Hack";
            // 
            // btnKeybindSpeedHack
            // 
            btnKeybindSpeedHack.Location = new Point(220, 110);
            btnKeybindSpeedHack.Name = "btnKeybindSpeedHack";
            btnKeybindSpeedHack.Size = new Size(100, 20);
            btnKeybindSpeedHack.TabIndex = 7;
            btnKeybindSpeedHack.Text = "None";
            btnKeybindSpeedHack.Click += BtnKeybindSpeedHack_Click;
            // 
            // lblKeybindAirStuck
            // 
            lblKeybindAirStuck.Location = new Point(15, 140);
            lblKeybindAirStuck.Name = "lblKeybindAirStuck";
            lblKeybindAirStuck.Size = new Size(200, 20);
            lblKeybindAirStuck.TabIndex = 8;
            lblKeybindAirStuck.Text = "Air Stuck";
            // 
            // btnKeybindAirStuck
            // 
            btnKeybindAirStuck.Location = new Point(220, 140);
            btnKeybindAirStuck.Name = "btnKeybindAirStuck";
            btnKeybindAirStuck.Size = new Size(100, 20);
            btnKeybindAirStuck.TabIndex = 9;
            btnKeybindAirStuck.Text = "None";
            btnKeybindAirStuck.Click += BtnKeybindAirStuck_Click;
            // 
            // lblKeybindHideActor
            // 
            lblKeybindHideActor.Location = new Point(15, 170);
            lblKeybindHideActor.Name = "lblKeybindHideActor";
            lblKeybindHideActor.Size = new Size(200, 20);
            lblKeybindHideActor.TabIndex = 10;
            lblKeybindHideActor.Text = "Hide Actor";
            // 
            // btnKeybindHideActor
            // 
            btnKeybindHideActor.Location = new Point(220, 170);
            btnKeybindHideActor.Name = "btnKeybindHideActor";
            btnKeybindHideActor.Size = new Size(100, 20);
            btnKeybindHideActor.TabIndex = 11;
            btnKeybindHideActor.Text = "None";
            btnKeybindHideActor.Click += BtnKeybindHideActor_Click;
            // 
            // lblKeybindToggleEnemyDistance
            // 
            lblKeybindToggleEnemyDistance.Location = new Point(15, 200);
            lblKeybindToggleEnemyDistance.Name = "lblKeybindToggleEnemyDistance";
            lblKeybindToggleEnemyDistance.Size = new Size(200, 20);
            lblKeybindToggleEnemyDistance.TabIndex = 12;
            lblKeybindToggleEnemyDistance.Text = "Toggle Enemy Distance";
            // 
            // btnKeybindToggleEnemyDistance
            // 
            btnKeybindToggleEnemyDistance.Location = new Point(220, 200);
            btnKeybindToggleEnemyDistance.Name = "btnKeybindToggleEnemyDistance";
            btnKeybindToggleEnemyDistance.Size = new Size(100, 20);
            btnKeybindToggleEnemyDistance.TabIndex = 13;
            btnKeybindToggleEnemyDistance.Text = "F4";
            btnKeybindToggleEnemyDistance.Click += BtnKeybindToggleEnemyDistance_Click;
            // 
            // lblKeybindToggleMap
            // 
            lblKeybindToggleMap.Location = new Point(15, 230);
            lblKeybindToggleMap.Name = "lblKeybindToggleMap";
            lblKeybindToggleMap.Size = new Size(200, 20);
            lblKeybindToggleMap.TabIndex = 14;
            lblKeybindToggleMap.Text = "Toggle Map";
            // 
            // btnKeybindToggleMap
            // 
            btnKeybindToggleMap.Location = new Point(220, 230);
            btnKeybindToggleMap.Name = "btnKeybindToggleMap";
            btnKeybindToggleMap.Size = new Size(100, 20);
            btnKeybindToggleMap.TabIndex = 15;
            btnKeybindToggleMap.Text = "F5";
            btnKeybindToggleMap.Click += BtnKeybindToggleMap_Click;
            // 
            // lblKeybindToggleFullscreen
            // 
            lblKeybindToggleFullscreen.Location = new Point(15, 260);
            lblKeybindToggleFullscreen.Name = "lblKeybindToggleFullscreen";
            lblKeybindToggleFullscreen.Size = new Size(200, 20);
            lblKeybindToggleFullscreen.TabIndex = 16;
            lblKeybindToggleFullscreen.Text = "Toggle Fullscreen";
            // 
            // btnKeybindToggleFullscreen
            // 
            btnKeybindToggleFullscreen.Location = new Point(220, 260);
            btnKeybindToggleFullscreen.Name = "btnKeybindToggleFullscreen";
            btnKeybindToggleFullscreen.Size = new Size(100, 20);
            btnKeybindToggleFullscreen.TabIndex = 17;
            btnKeybindToggleFullscreen.Text = "F11";
            btnKeybindToggleFullscreen.Click += BtnKeybindToggleFullscreen_Click;
            // 
            // lblStatusSetInteractionDistances
            // 
            lblStatusSetInteractionDistances.AutoSize = true;
            lblStatusSetInteractionDistances.Location = new Point(330, 50);
            lblStatusSetInteractionDistances.Name = "lblStatusSetInteractionDistances";
            lblStatusSetInteractionDistances.Size = new Size(28, 15);
            lblStatusSetInteractionDistances.TabIndex = 19;
            lblStatusSetInteractionDistances.Text = "OFF";
            // 
            // lblStatusAllowShootingInMainBase
            // 
            lblStatusAllowShootingInMainBase.AutoSize = true;
            lblStatusAllowShootingInMainBase.Location = new Point(330, 80);
            lblStatusAllowShootingInMainBase.Name = "lblStatusAllowShootingInMainBase";
            lblStatusAllowShootingInMainBase.Size = new Size(28, 15);
            lblStatusAllowShootingInMainBase.TabIndex = 20;
            lblStatusAllowShootingInMainBase.Text = "OFF";
            // 
            // lblStatusSpeedHack
            // 
            lblStatusSpeedHack.AutoSize = true;
            lblStatusSpeedHack.Location = new Point(330, 110);
            lblStatusSpeedHack.Name = "lblStatusSpeedHack";
            lblStatusSpeedHack.Size = new Size(28, 15);
            lblStatusSpeedHack.TabIndex = 21;
            lblStatusSpeedHack.Text = "OFF";
            // 
            // lblStatusAirStuck
            // 
            lblStatusAirStuck.AutoSize = true;
            lblStatusAirStuck.Location = new Point(330, 140);
            lblStatusAirStuck.Name = "lblStatusAirStuck";
            lblStatusAirStuck.Size = new Size(28, 15);
            lblStatusAirStuck.TabIndex = 22;
            lblStatusAirStuck.Text = "OFF";
            // 
            // lblStatusHideActor
            // 
            lblStatusHideActor.AutoSize = true;
            lblStatusHideActor.Location = new Point(330, 170);
            lblStatusHideActor.Name = "lblStatusHideActor";
            lblStatusHideActor.Size = new Size(28, 15);
            lblStatusHideActor.TabIndex = 23;
            lblStatusHideActor.Text = "OFF";
            // 
            // lblStatusToggleEnemyDistance
            // 
            lblStatusToggleEnemyDistance.AutoSize = true;
            lblStatusToggleEnemyDistance.Location = new Point(330, 200);
            lblStatusToggleEnemyDistance.Name = "lblStatusToggleEnemyDistance";
            lblStatusToggleEnemyDistance.Size = new Size(28, 15);
            lblStatusToggleEnemyDistance.TabIndex = 24;
            lblStatusToggleEnemyDistance.Text = "OFF";
            // 
            // lblKeybindDumpNames
            // 
            lblKeybindDumpNames.Location = new Point(15, 290);
            lblKeybindDumpNames.Name = "lblKeybindDumpNames";
            lblKeybindDumpNames.Size = new Size(200, 20);
            lblKeybindDumpNames.TabIndex = 18;
            lblKeybindDumpNames.Text = "Dump Names";
            // 
            // btnKeybindDumpNames
            // 
            btnKeybindDumpNames.Location = new Point(220, 290);
            btnKeybindDumpNames.Name = "btnKeybindDumpNames";
            btnKeybindDumpNames.Size = new Size(100, 20);
            btnKeybindDumpNames.TabIndex = 19;
            btnKeybindDumpNames.Text = "F6";
            btnKeybindDumpNames.Click += BtnKeybindDumpNames_Click;
            // 
            // lblKeybindZoomIn
            // 
            lblKeybindZoomIn.Location = new Point(15, 320);
            lblKeybindZoomIn.Name = "lblKeybindZoomIn";
            lblKeybindZoomIn.Size = new Size(200, 20);
            lblKeybindZoomIn.TabIndex = 20;
            lblKeybindZoomIn.Text = "Zoom In";
            // 
            // btnKeybindZoomIn
            // 
            btnKeybindZoomIn.Location = new Point(220, 320);
            btnKeybindZoomIn.Name = "btnKeybindZoomIn";
            btnKeybindZoomIn.Size = new Size(100, 20);
            btnKeybindZoomIn.TabIndex = 21;
            btnKeybindZoomIn.Text = "Up";
            btnKeybindZoomIn.Click += BtnKeybindZoomIn_Click;
            // 
            // lblKeybindZoomOut
            // 
            lblKeybindZoomOut.Location = new Point(15, 350);
            lblKeybindZoomOut.Name = "lblKeybindZoomOut";
            lblKeybindZoomOut.Size = new Size(200, 20);
            lblKeybindZoomOut.TabIndex = 22;
            lblKeybindZoomOut.Text = "Zoom Out";
            // 
            // btnKeybindZoomOut
            // 
            btnKeybindZoomOut.Location = new Point(220, 350);
            btnKeybindZoomOut.Name = "btnKeybindZoomOut";
            btnKeybindZoomOut.Size = new Size(100, 20);
            btnKeybindZoomOut.TabIndex = 23;
            btnKeybindZoomOut.Text = "Down";
            btnKeybindZoomOut.Click += BtnKeybindZoomOut_Click;
            // 
            // tabRadar
            // 
            tabRadar.Controls.Add(ticketsPanel);
            tabRadar.Controls.Add(chkMapFree);
            tabRadar.Controls.Add(grpMapSetup);
            tabRadar.Location = new Point(4, 24);
            tabRadar.Name = "tabRadar";
            tabRadar.Padding = new Padding(3);
            tabRadar.Size = new Size(1592, 872);
            tabRadar.TabIndex = 0;
            tabRadar.Text = "Radar";
            tabRadar.UseVisualStyleBackColor = true;
            // 
            // ticketsPanel
            // 
            ticketsPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ticketsPanel.BackColor = SystemColors.ScrollBar;
            ticketsPanel.BorderStyle = BorderStyle.FixedSingle;
            ticketsPanel.Location = new Point(9, 836);
            ticketsPanel.Name = "ticketsPanel";
            ticketsPanel.Size = new Size(285, 30);
            ticketsPanel.TabIndex = 12;
            // 
            // grpMapSetup
            // 
            grpMapSetup.Controls.Add(btnApplyMapScale);
            grpMapSetup.Controls.Add(txtMapSetupScale);
            grpMapSetup.Controls.Add(lblMapScale);
            grpMapSetup.Controls.Add(txtMapSetupY);
            grpMapSetup.Controls.Add(lblMapXY);
            grpMapSetup.Controls.Add(txtMapSetupX);
            grpMapSetup.Controls.Add(lblMapCoords);
            grpMapSetup.Location = new Point(9, 45);
            grpMapSetup.Name = "grpMapSetup";
            grpMapSetup.Padding = new Padding(15);
            grpMapSetup.Size = new Size(400, 250);
            grpMapSetup.TabIndex = 11;
            grpMapSetup.TabStop = false;
            grpMapSetup.Text = "Map Setup";
            grpMapSetup.Visible = false;
            // 
            // lblMapScale
            // 
            lblMapScale.AutoSize = true;
            lblMapScale.Font = new Font("Segoe UI", 9F);
            lblMapScale.Location = new Point(15, 90);
            lblMapScale.Name = "lblMapScale";
            lblMapScale.Size = new Size(34, 15);
            lblMapScale.TabIndex = 19;
            lblMapScale.Text = "Scale";
            // 
            // lblMapXY
            // 
            lblMapXY.AutoSize = true;
            lblMapXY.Font = new Font("Segoe UI", 9F);
            lblMapXY.Location = new Point(15, 60);
            lblMapXY.Name = "lblMapXY";
            lblMapXY.Size = new Size(24, 15);
            lblMapXY.TabIndex = 20;
            lblMapXY.Text = "X,Y";
            // 
            // lblMapCoords
            // 
            lblMapCoords.AutoSize = true;
            lblMapCoords.Font = new Font("Segoe UI", 9F);
            lblMapCoords.Location = new Point(15, 30);
            lblMapCoords.Name = "lblMapCoords";
            lblMapCoords.Size = new Size(98, 15);
            lblMapCoords.TabIndex = 21;
            lblMapCoords.Text = "Map Coordinates";
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabRadar);
            tabControl.Controls.Add(tabSettings);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1600, 900);
            tabControl.TabIndex = 8;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1600, 900);
            Controls.Add(tabControl);
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "Squad DMA";
            ((System.ComponentModel.ISupportInitialize)trkUIScale).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkAimLength).EndInit();
            tabSettings.ResumeLayout(false);
            grpConfig.ResumeLayout(false);
            grpUserInterface.ResumeLayout(false);
            grpUserInterface.PerformLayout();
            grpRadar.ResumeLayout(false);
            grpRadar.PerformLayout();
            grpLocalSoldier.ResumeLayout(false);
            grpLocalSoldier.PerformLayout();
            grpKeybinds.ResumeLayout(false);
            grpKeybinds.PerformLayout();
            tabRadar.ResumeLayout(false);
            tabRadar.PerformLayout();
            grpMapSetup.ResumeLayout(false);
            grpMapSetup.PerformLayout();
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ColorDialog colDialog;
        private ToolTip toolTip;
        private TabPage tabSettings;
        private GroupBox grpConfig;
        private GroupBox grpUserInterface;
        private TrackBar trkAimLength;
        private Label lblAimline;
        private Label lblUIScale;
        private TrackBar trkUIScale;
        private CheckBox chkShowEnemyDistance;
        private Button btnDumpNames;
        private GroupBox grpRadar;
        private Button btnRestartRadar;
        private CheckBox chkShowMapSetup;
        private Button btnToggleMap;
        private TabPage tabRadar;
        private GroupBox grpMapSetup;
        private Button btnApplyMapScale;
        private CheckBox chkMapFree;
        private TextBox txtMapSetupScale;
        private Label lblMapScale;
        private TextBox txtMapSetupY;
        private Label lblMapXY;
        private TextBox txtMapSetupX;
        private Label lblMapCoords;
        private TabControl tabControl;
        private Panel ticketsPanel;
        private GroupBox grpLocalSoldier;
        private CheckBox chkDisableSuppression;
        private CheckBox chkSetInteractionDistances;
        private CheckBox chkAllowShootingInMainBase;
        private CheckBox chkSpeedHack;
        private CheckBox chkAirStuck;
        private CheckBox chkHideActor;
        private GroupBox grpKeybinds;
        private Button btnKeybindSetInteractionDistances;
        private Button btnKeybindAllowShootingInMainBase;
        private Button btnKeybindSpeedHack;
        private Button btnKeybindAirStuck;
        private Button btnKeybindHideActor;
        private Button btnKeybindToggleEnemyDistance;
        private Button btnKeybindToggleMap;
        private Button btnKeybindToggleFullscreen;
        private Label lblKeybindSetInteractionDistances;
        private Label lblKeybindAllowShootingInMainBase;
        private Label lblKeybindSpeedHack;
        private Label lblKeybindAirStuck;
        private Label lblKeybindHideActor;
        private Label lblKeybindToggleEnemyDistance;
        private Label lblKeybindToggleMap;
        private Label lblKeybindToggleFullscreen;
        private Label lblStatusSetInteractionDistances;
        private Label lblStatusAllowShootingInMainBase;
        private Label lblStatusSpeedHack;
        private Label lblStatusAirStuck;
        private Label lblStatusHideActor;
        private Label lblStatusToggleEnemyDistance;
        private Label lblKeybindDumpNames;
        private Button btnKeybindDumpNames;
        private Label lblKeybindZoomIn;
        private Button btnKeybindZoomIn;
        private Label lblKeybindZoomOut;
        private Button btnKeybindZoomOut;

        private void BtnKeybindZoomIn_Click(object sender, EventArgs e)
        {
            StartKeybindCapture(btnKeybindZoomIn);
        }

        private void BtnKeybindZoomOut_Click(object sender, EventArgs e)
        {
            StartKeybindCapture(btnKeybindZoomOut);
        }
    }
}
