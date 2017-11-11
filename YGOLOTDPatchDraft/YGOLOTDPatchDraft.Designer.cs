namespace YGOPRODraft
{
    partial class YGOLOTDPatchDraft
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
			this.txtDebugOut = new System.Windows.Forms.RichTextBox();
			this.btnExtractGameFiles = new System.Windows.Forms.Button();
			this.btnUnpackDecksAndPacks = new System.Windows.Forms.Button();
			this.btnPatchBattlePack = new System.Windows.Forms.Button();
			this.btnPackGame = new System.Windows.Forms.Button();
			this.btnCopyToGameDirectory = new System.Windows.Forms.Button();
			this.btnPatchAIDraftDecks = new System.Windows.Forms.Button();
			this.btnExtractSaveGameDeck = new System.Windows.Forms.Button();
			this.chkJSONRarity = new System.Windows.Forms.CheckBox();
			this.btnPatchPackCopy = new System.Windows.Forms.Button();
			this.chkListBoxDecks = new System.Windows.Forms.CheckedListBox();
			this.chkListBoxPacks = new System.Windows.Forms.CheckedListBox();
			this.txtFilterDecks = new System.Windows.Forms.TextBox();
			this.txtFilterPacks = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.chkAllDecks = new System.Windows.Forms.CheckBox();
			this.chkAllPacks = new System.Windows.Forms.CheckBox();
			this.chkOnlyShowChosenDecks = new System.Windows.Forms.CheckBox();
			this.chkOnlyShowChosenPacks = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtDebugOut
			// 
			this.txtDebugOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtDebugOut.HideSelection = false;
			this.txtDebugOut.Location = new System.Drawing.Point(3, 248);
			this.txtDebugOut.Name = "txtDebugOut";
			this.txtDebugOut.Size = new System.Drawing.Size(891, 273);
			this.txtDebugOut.TabIndex = 8;
			this.txtDebugOut.Text = "";
			// 
			// btnExtractGameFiles
			// 
			this.btnExtractGameFiles.Location = new System.Drawing.Point(12, 12);
			this.btnExtractGameFiles.Name = "btnExtractGameFiles";
			this.btnExtractGameFiles.Size = new System.Drawing.Size(228, 31);
			this.btnExtractGameFiles.TabIndex = 9;
			this.btnExtractGameFiles.Text = "Extract YGO: LOTD Game Files";
			this.btnExtractGameFiles.UseVisualStyleBackColor = true;
			this.btnExtractGameFiles.Click += new System.EventHandler(this.btnExtractGameFiles_Click);
			// 
			// btnUnpackDecksAndPacks
			// 
			this.btnUnpackDecksAndPacks.Location = new System.Drawing.Point(12, 49);
			this.btnUnpackDecksAndPacks.Name = "btnUnpackDecksAndPacks";
			this.btnUnpackDecksAndPacks.Size = new System.Drawing.Size(228, 31);
			this.btnUnpackDecksAndPacks.TabIndex = 10;
			this.btnUnpackDecksAndPacks.Text = "Unpack Decks/Packs";
			this.btnUnpackDecksAndPacks.UseVisualStyleBackColor = true;
			this.btnUnpackDecksAndPacks.Click += new System.EventHandler(this.btnUnpackDecksAndPacks_Click);
			// 
			// btnPatchBattlePack
			// 
			this.btnPatchBattlePack.Location = new System.Drawing.Point(12, 86);
			this.btnPatchBattlePack.Name = "btnPatchBattlePack";
			this.btnPatchBattlePack.Size = new System.Drawing.Size(107, 40);
			this.btnPatchBattlePack.TabIndex = 11;
			this.btnPatchBattlePack.Text = "Patch bpack_BattlePack";
			this.btnPatchBattlePack.UseVisualStyleBackColor = true;
			this.btnPatchBattlePack.Click += new System.EventHandler(this.btnPatchBattlePack_Click);
			// 
			// btnPackGame
			// 
			this.btnPackGame.Location = new System.Drawing.Point(12, 132);
			this.btnPackGame.Name = "btnPackGame";
			this.btnPackGame.Size = new System.Drawing.Size(228, 31);
			this.btnPackGame.TabIndex = 12;
			this.btnPackGame.Text = "Pack YGO_DATA files";
			this.btnPackGame.UseVisualStyleBackColor = true;
			this.btnPackGame.Click += new System.EventHandler(this.btnPackGame_Click);
			// 
			// btnCopyToGameDirectory
			// 
			this.btnCopyToGameDirectory.Location = new System.Drawing.Point(12, 169);
			this.btnCopyToGameDirectory.Name = "btnCopyToGameDirectory";
			this.btnCopyToGameDirectory.Size = new System.Drawing.Size(228, 31);
			this.btnCopyToGameDirectory.TabIndex = 13;
			this.btnCopyToGameDirectory.Text = "Copy Patched Files to Game Directory";
			this.btnCopyToGameDirectory.UseVisualStyleBackColor = true;
			this.btnCopyToGameDirectory.Click += new System.EventHandler(this.btnCopyToGameDirectory_Click);
			// 
			// btnPatchAIDraftDecks
			// 
			this.btnPatchAIDraftDecks.Location = new System.Drawing.Point(125, 86);
			this.btnPatchAIDraftDecks.Name = "btnPatchAIDraftDecks";
			this.btnPatchAIDraftDecks.Size = new System.Drawing.Size(115, 40);
			this.btnPatchAIDraftDecks.TabIndex = 14;
			this.btnPatchAIDraftDecks.Text = "Patch AI Draft Decks";
			this.btnPatchAIDraftDecks.UseVisualStyleBackColor = true;
			this.btnPatchAIDraftDecks.Click += new System.EventHandler(this.btnPatchAIDraftDecks_Click);
			// 
			// btnExtractSaveGameDeck
			// 
			this.btnExtractSaveGameDeck.Location = new System.Drawing.Point(246, 49);
			this.btnExtractSaveGameDeck.Name = "btnExtractSaveGameDeck";
			this.btnExtractSaveGameDeck.Size = new System.Drawing.Size(189, 31);
			this.btnExtractSaveGameDeck.TabIndex = 15;
			this.btnExtractSaveGameDeck.Text = "Extract Draft Deck from LOTD Save";
			this.btnExtractSaveGameDeck.UseVisualStyleBackColor = true;
			this.btnExtractSaveGameDeck.Click += new System.EventHandler(this.btnExtractSaveGameDeck_Click);
			// 
			// chkJSONRarity
			// 
			this.chkJSONRarity.AutoSize = true;
			this.chkJSONRarity.Checked = true;
			this.chkJSONRarity.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkJSONRarity.Location = new System.Drawing.Point(246, 99);
			this.chkJSONRarity.Name = "chkJSONRarity";
			this.chkJSONRarity.Size = new System.Drawing.Size(149, 17);
			this.chkJSONRarity.TabIndex = 16;
			this.chkJSONRarity.Text = "Simulate Rarity (only .json)";
			this.chkJSONRarity.UseVisualStyleBackColor = true;
			this.chkJSONRarity.CheckedChanged += new System.EventHandler(this.chkJSONRarity_CheckedChanged);
			// 
			// btnPatchPackCopy
			// 
			this.btnPatchPackCopy.BackColor = System.Drawing.Color.Maroon;
			this.btnPatchPackCopy.ForeColor = System.Drawing.SystemColors.Control;
			this.btnPatchPackCopy.Location = new System.Drawing.Point(12, 206);
			this.btnPatchPackCopy.Name = "btnPatchPackCopy";
			this.btnPatchPackCopy.Size = new System.Drawing.Size(228, 31);
			this.btnPatchPackCopy.TabIndex = 17;
			this.btnPatchPackCopy.Text = "Patch, Pack, Copy all";
			this.btnPatchPackCopy.UseVisualStyleBackColor = false;
			this.btnPatchPackCopy.Click += new System.EventHandler(this.btnPatchPackCopy_Click);
			// 
			// chkListBoxDecks
			// 
			this.chkListBoxDecks.CheckOnClick = true;
			this.chkListBoxDecks.FormattingEnabled = true;
			this.chkListBoxDecks.HorizontalScrollbar = true;
			this.chkListBoxDecks.Location = new System.Drawing.Point(441, 49);
			this.chkListBoxDecks.Name = "chkListBoxDecks";
			this.chkListBoxDecks.Size = new System.Drawing.Size(213, 184);
			this.chkListBoxDecks.TabIndex = 18;
			this.chkListBoxDecks.SelectedIndexChanged += new System.EventHandler(this.chkListBoxDecks_SelectedIndexChanged);
			// 
			// chkListBoxPacks
			// 
			this.chkListBoxPacks.CheckOnClick = true;
			this.chkListBoxPacks.FormattingEnabled = true;
			this.chkListBoxPacks.HorizontalScrollbar = true;
			this.chkListBoxPacks.Location = new System.Drawing.Point(672, 49);
			this.chkListBoxPacks.Name = "chkListBoxPacks";
			this.chkListBoxPacks.Size = new System.Drawing.Size(213, 184);
			this.chkListBoxPacks.TabIndex = 19;
			this.chkListBoxPacks.SelectedIndexChanged += new System.EventHandler(this.chkListBoxPacks_SelectedIndexChanged);
			// 
			// txtFilterDecks
			// 
			this.txtFilterDecks.Location = new System.Drawing.Point(554, 23);
			this.txtFilterDecks.Name = "txtFilterDecks";
			this.txtFilterDecks.Size = new System.Drawing.Size(100, 20);
			this.txtFilterDecks.TabIndex = 20;
			this.txtFilterDecks.TextChanged += new System.EventHandler(this.txtFilterDecks_TextChanged);
			// 
			// txtFilterPacks
			// 
			this.txtFilterPacks.Location = new System.Drawing.Point(785, 23);
			this.txtFilterPacks.Name = "txtFilterPacks";
			this.txtFilterPacks.Size = new System.Drawing.Size(100, 20);
			this.txtFilterPacks.TabIndex = 21;
			this.txtFilterPacks.TextChanged += new System.EventHandler(this.txtFilterPacks_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(516, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 22;
			this.label1.Text = "Filter:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(747, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 13);
			this.label2.TabIndex = 23;
			this.label2.Text = "Filter:";
			// 
			// chkAllDecks
			// 
			this.chkAllDecks.AutoSize = true;
			this.chkAllDecks.Location = new System.Drawing.Point(444, 29);
			this.chkAllDecks.Name = "chkAllDecks";
			this.chkAllDecks.Size = new System.Drawing.Size(37, 17);
			this.chkAllDecks.TabIndex = 24;
			this.chkAllDecks.Text = "All";
			this.chkAllDecks.UseVisualStyleBackColor = true;
			this.chkAllDecks.CheckedChanged += new System.EventHandler(this.chkAllDecks_CheckedChanged);
			// 
			// chkAllPacks
			// 
			this.chkAllPacks.AutoSize = true;
			this.chkAllPacks.Location = new System.Drawing.Point(675, 29);
			this.chkAllPacks.Name = "chkAllPacks";
			this.chkAllPacks.Size = new System.Drawing.Size(37, 17);
			this.chkAllPacks.TabIndex = 25;
			this.chkAllPacks.Text = "All";
			this.chkAllPacks.UseVisualStyleBackColor = true;
			this.chkAllPacks.CheckedChanged += new System.EventHandler(this.chkAllPacks_CheckedChanged);
			// 
			// chkOnlyShowChosenDecks
			// 
			this.chkOnlyShowChosenDecks.AutoSize = true;
			this.chkOnlyShowChosenDecks.Location = new System.Drawing.Point(540, 6);
			this.chkOnlyShowChosenDecks.Name = "chkOnlyShowChosenDecks";
			this.chkOnlyShowChosenDecks.Size = new System.Drawing.Size(121, 17);
			this.chkOnlyShowChosenDecks.TabIndex = 26;
			this.chkOnlyShowChosenDecks.Text = "Show only Checked";
			this.chkOnlyShowChosenDecks.UseVisualStyleBackColor = true;
			this.chkOnlyShowChosenDecks.CheckedChanged += new System.EventHandler(this.chkOnlyShowChosenDecks_CheckedChanged);
			// 
			// chkOnlyShowChosenPacks
			// 
			this.chkOnlyShowChosenPacks.AutoSize = true;
			this.chkOnlyShowChosenPacks.Location = new System.Drawing.Point(770, 6);
			this.chkOnlyShowChosenPacks.Name = "chkOnlyShowChosenPacks";
			this.chkOnlyShowChosenPacks.Size = new System.Drawing.Size(121, 17);
			this.chkOnlyShowChosenPacks.TabIndex = 27;
			this.chkOnlyShowChosenPacks.Text = "Show only Checked";
			this.chkOnlyShowChosenPacks.UseVisualStyleBackColor = true;
			this.chkOnlyShowChosenPacks.CheckedChanged += new System.EventHandler(this.chkOnlyShowChosenPacks_CheckedChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(437, 6);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(58, 20);
			this.label4.TabIndex = 29;
			this.label4.Text = "Decks:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(671, 6);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 20);
			this.label5.TabIndex = 30;
			this.label5.Text = "Packs:";
			// 
			// YGOLOTDPatchDraft
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(897, 519);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.chkOnlyShowChosenPacks);
			this.Controls.Add(this.chkOnlyShowChosenDecks);
			this.Controls.Add(this.chkAllPacks);
			this.Controls.Add(this.chkAllDecks);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtFilterPacks);
			this.Controls.Add(this.txtFilterDecks);
			this.Controls.Add(this.chkListBoxPacks);
			this.Controls.Add(this.chkListBoxDecks);
			this.Controls.Add(this.btnPatchPackCopy);
			this.Controls.Add(this.chkJSONRarity);
			this.Controls.Add(this.btnExtractSaveGameDeck);
			this.Controls.Add(this.btnPatchAIDraftDecks);
			this.Controls.Add(this.btnCopyToGameDirectory);
			this.Controls.Add(this.btnPackGame);
			this.Controls.Add(this.btnPatchBattlePack);
			this.Controls.Add(this.btnUnpackDecksAndPacks);
			this.Controls.Add(this.btnExtractGameFiles);
			this.Controls.Add(this.txtDebugOut);
			this.Name = "YGOLOTDPatchDraft";
			this.Text = "Yu-Gi-Oh: Legacy of The Duelist Draft Injector";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
		public System.Windows.Forms.RichTextBox txtDebugOut;
		private System.Windows.Forms.Button btnExtractGameFiles;
		private System.Windows.Forms.Button btnUnpackDecksAndPacks;
		private System.Windows.Forms.Button btnPatchBattlePack;
		private System.Windows.Forms.Button btnPackGame;
		private System.Windows.Forms.Button btnCopyToGameDirectory;
		private System.Windows.Forms.Button btnPatchAIDraftDecks;
		private System.Windows.Forms.Button btnExtractSaveGameDeck;
		private System.Windows.Forms.CheckBox chkJSONRarity;
		private System.Windows.Forms.Button btnPatchPackCopy;
		private System.Windows.Forms.CheckedListBox chkListBoxDecks;
		private System.Windows.Forms.CheckedListBox chkListBoxPacks;
		private System.Windows.Forms.TextBox txtFilterDecks;
		private System.Windows.Forms.TextBox txtFilterPacks;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox chkOnlyShowChosenDecks;
		private System.Windows.Forms.CheckBox chkOnlyShowChosenPacks;
		private System.Windows.Forms.CheckBox chkAllPacks;
		private System.Windows.Forms.CheckBox chkAllDecks;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
	}
}

