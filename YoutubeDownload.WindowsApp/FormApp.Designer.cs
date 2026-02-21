namespace WindowsApp
{
    partial class FormApp
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
            labelUrlVideo = new Label();
            textBoxUrlVideo = new TextBox();
            labelOutput = new Label();
            textBoxOutput = new TextBox();
            buttonPlay = new Button();
            checkBoxConverterMp3 = new CheckBox();
            checkBoxAutoPlay = new CheckBox();
            buttonDownloadInfo = new Button();
            dataGridView = new DataGridView();
            buttonDownload = new Button();
            buttonAtualizar = new Button();
            buttonOk = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // labelUrlVideo
            // 
            labelUrlVideo.AutoSize = true;
            labelUrlVideo.Location = new Point(24, 9);
            labelUrlVideo.Name = "labelUrlVideo";
            labelUrlVideo.Size = new Size(78, 15);
            labelUrlVideo.TabIndex = 0;
            labelUrlVideo.Text = "URL do Vídeo";
            // 
            // textBoxUrlVideo
            // 
            textBoxUrlVideo.Font = new Font("Segoe UI", 9F);
            textBoxUrlVideo.Location = new Point(24, 30);
            textBoxUrlVideo.Multiline = true;
            textBoxUrlVideo.Name = "textBoxUrlVideo";
            textBoxUrlVideo.PlaceholderText = "Digite aqui a url do vídeo e tecle enter...";
            textBoxUrlVideo.Size = new Size(604, 25);
            textBoxUrlVideo.TabIndex = 1;
            textBoxUrlVideo.KeyDown += DownloadManifestInfoEnter;
            // 
            // labelOutput
            // 
            labelOutput.AutoSize = true;
            labelOutput.Location = new Point(24, 342);
            labelOutput.Name = "labelOutput";
            labelOutput.Size = new Size(102, 15);
            labelOutput.TabIndex = 4;
            labelOutput.Text = "Arquivo salvo em:";
            // 
            // textBoxOutput
            // 
            textBoxOutput.Location = new Point(24, 360);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.Size = new Size(748, 25);
            textBoxOutput.TabIndex = 5;
            // 
            // buttonPlay
            // 
            buttonPlay.FlatStyle = FlatStyle.System;
            buttonPlay.Location = new Point(490, 401);
            buttonPlay.Name = "buttonPlay";
            buttonPlay.Size = new Size(138, 30);
            buttonPlay.TabIndex = 6;
            buttonPlay.Text = "Play";
            buttonPlay.UseVisualStyleBackColor = true;
            buttonPlay.Click += Play;
            // 
            // checkBoxConverterMp3
            // 
            checkBoxConverterMp3.Location = new Point(272, 406);
            checkBoxConverterMp3.Name = "checkBoxConverterMp3";
            checkBoxConverterMp3.Size = new Size(131, 23);
            checkBoxConverterMp3.TabIndex = 8;
            checkBoxConverterMp3.Text = "Converter Para Mp3";
            checkBoxConverterMp3.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoPlay
            // 
            checkBoxAutoPlay.Location = new Point(409, 406);
            checkBoxAutoPlay.Name = "checkBoxAutoPlay";
            checkBoxAutoPlay.Size = new Size(75, 23);
            checkBoxAutoPlay.TabIndex = 9;
            checkBoxAutoPlay.Text = "AutoPlay";
            checkBoxAutoPlay.UseVisualStyleBackColor = true;
            // 
            // buttonDownloadInfo
            // 
            buttonDownloadInfo.FlatStyle = FlatStyle.System;
            buttonDownloadInfo.Location = new Point(634, 30);
            buttonDownloadInfo.Name = "buttonDownloadInfo";
            buttonDownloadInfo.Size = new Size(99, 25);
            buttonDownloadInfo.TabIndex = 2;
            buttonDownloadInfo.Text = "Clique aqui";
            buttonDownloadInfo.UseVisualStyleBackColor = true;
            buttonDownloadInfo.Click += DownloadManifestInfo;
            // 
            // dataGridView
            // 
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(24, 66);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(748, 273);
            dataGridView.TabIndex = 11;
            // 
            // buttonDownload
            // 
            buttonDownload.FlatStyle = FlatStyle.System;
            buttonDownload.Location = new Point(634, 401);
            buttonDownload.Name = "buttonDownload";
            buttonDownload.Size = new Size(138, 30);
            buttonDownload.TabIndex = 3;
            buttonDownload.Text = "Download";
            buttonDownload.UseVisualStyleBackColor = true;
            buttonDownload.Click += Download;
            // 
            // buttonAtualizar
            // 
            buttonAtualizar.BackColor = SystemColors.ButtonHighlight;
            buttonAtualizar.Image = YoutubeDownload.App.Properties.Resources.atualizar;
            buttonAtualizar.Location = new Point(739, 30);
            buttonAtualizar.Name = "buttonAtualizar";
            buttonAtualizar.Size = new Size(33, 25);
            buttonAtualizar.TabIndex = 13;
            buttonAtualizar.UseVisualStyleBackColor = false;
            buttonAtualizar.Click += Atualizar;
            // 
            // buttonOk
            // 
            buttonOk.BackColor = SystemColors.ButtonHighlight;
            buttonOk.Image = YoutubeDownload.App.Properties.Resources.verificar_cinza;
            buttonOk.Location = new Point(24, 401);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(33, 25);
            buttonOk.TabIndex = 14;
            buttonOk.UseVisualStyleBackColor = false;
            // 
            // FormApp
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonOk);
            Controls.Add(buttonAtualizar);
            Controls.Add(buttonDownload);
            Controls.Add(dataGridView);
            Controls.Add(buttonDownloadInfo);
            Controls.Add(checkBoxAutoPlay);
            Controls.Add(checkBoxConverterMp3);
            Controls.Add(buttonPlay);
            Controls.Add(textBoxOutput);
            Controls.Add(labelOutput);
            Controls.Add(textBoxUrlVideo);
            Controls.Add(labelUrlVideo);
            Name = "FormApp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Youtube Download";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelUrlVideo;
        private Label labelOutput;
        private TextBox textBoxUrlVideo;
        private TextBox textBoxOutput;
        private CheckBox checkBoxConverterMp3;
        private CheckBox checkBoxAutoPlay;
        private DataGridView dataGridView;
        private Button buttonPlay;
        private Button buttonDownloadInfo;
        private Button buttonDownload;
        private Button buttonAtualizar;
        private Button buttonOk;
    }
}
