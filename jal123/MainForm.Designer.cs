namespace Jal123.GUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.cbxSvnRepository = new System.Windows.Forms.ComboBox();
            this.txtTargetDirectory = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnTargetDirectory = new System.Windows.Forms.Button();
            this.dlgTargetDirectoryBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.txtNumbers = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.pbExport = new System.Windows.Forms.ProgressBar();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.exportSvnBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.tbExportWay = new System.Windows.Forms.TabControl();
            this.tabPageSvnRevision = new System.Windows.Forms.TabPage();
            this.tabPageZentaoBuild = new System.Windows.Forms.TabPage();
            this.cbxZentaoBuild = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbxZentaoProject = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnRequestZentaoProduct = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxZentaoProduct = new System.Windows.Forms.ComboBox();
            this.tabPageZentaoBug = new System.Windows.Forms.TabPage();
            this.txtZentaoBug = new System.Windows.Forms.TextBox();
            this.tabPageZentaoStory = new System.Windows.Forms.TabPage();
            this.txtZentaoStory = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.tbExportWay.SuspendLayout();
            this.tabPageSvnRevision.SuspendLayout();
            this.tabPageZentaoBuild.SuspendLayout();
            this.tabPageZentaoBug.SuspendLayout();
            this.tabPageZentaoStory.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cbxSvnRepository
            // 
            this.cbxSvnRepository.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSvnRepository.Items.AddRange(new object[] {
            resources.GetString("cbxSvnRepository.Items")});
            resources.ApplyResources(this.cbxSvnRepository, "cbxSvnRepository");
            this.cbxSvnRepository.Name = "cbxSvnRepository";
            // 
            // txtTargetDirectory
            // 
            this.txtTargetDirectory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtTargetDirectory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            resources.ApplyResources(this.txtTargetDirectory, "txtTargetDirectory");
            this.txtTargetDirectory.Name = "txtTargetDirectory";
            this.txtTargetDirectory.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btnTargetDirectory
            // 
            resources.ApplyResources(this.btnTargetDirectory, "btnTargetDirectory");
            this.btnTargetDirectory.Name = "btnTargetDirectory";
            this.btnTargetDirectory.UseVisualStyleBackColor = true;
            this.btnTargetDirectory.Click += new System.EventHandler(this.btnTargetDirectory_Click);
            // 
            // txtNumbers
            // 
            this.txtNumbers.AcceptsReturn = true;
            resources.ApplyResources(this.txtNumbers, "txtNumbers");
            this.txtNumbers.Name = "txtNumbers";
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnExport
            // 
            resources.ApplyResources(this.btnExport, "btnExport");
            this.btnExport.Name = "btnExport";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // pbExport
            // 
            resources.ApplyResources(this.pbExport, "pbExport");
            this.pbExport.Name = "pbExport";
            this.pbExport.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lblProgress
            // 
            resources.ApplyResources(this.lblProgress, "lblProgress");
            this.lblProgress.Name = "lblProgress";
            // 
            // exportSvnBackgroundWorker
            // 
            this.exportSvnBackgroundWorker.WorkerReportsProgress = true;
            // 
            // tbExportWay
            // 
            this.tbExportWay.Controls.Add(this.tabPageSvnRevision);
            this.tbExportWay.Controls.Add(this.tabPageZentaoBuild);
            this.tbExportWay.Controls.Add(this.tabPageZentaoBug);
            this.tbExportWay.Controls.Add(this.tabPageZentaoStory);
            resources.ApplyResources(this.tbExportWay, "tbExportWay");
            this.tbExportWay.Name = "tbExportWay";
            this.tbExportWay.SelectedIndex = 0;
            this.tbExportWay.Selected += new System.Windows.Forms.TabControlEventHandler(this.tbExportWay_Selected);
            // 
            // tabPageSvnRevision
            // 
            this.tabPageSvnRevision.Controls.Add(this.txtNumbers);
            resources.ApplyResources(this.tabPageSvnRevision, "tabPageSvnRevision");
            this.tabPageSvnRevision.Name = "tabPageSvnRevision";
            this.tabPageSvnRevision.UseVisualStyleBackColor = true;
            // 
            // tabPageZentaoBuild
            // 
            this.tabPageZentaoBuild.Controls.Add(this.cbxZentaoBuild);
            this.tabPageZentaoBuild.Controls.Add(this.label6);
            this.tabPageZentaoBuild.Controls.Add(this.cbxZentaoProject);
            this.tabPageZentaoBuild.Controls.Add(this.label5);
            this.tabPageZentaoBuild.Controls.Add(this.btnRequestZentaoProduct);
            this.tabPageZentaoBuild.Controls.Add(this.label4);
            this.tabPageZentaoBuild.Controls.Add(this.cbxZentaoProduct);
            resources.ApplyResources(this.tabPageZentaoBuild, "tabPageZentaoBuild");
            this.tabPageZentaoBuild.Name = "tabPageZentaoBuild";
            this.tabPageZentaoBuild.UseVisualStyleBackColor = true;
            // 
            // cbxZentaoBuild
            // 
            this.cbxZentaoBuild.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxZentaoBuild.FormattingEnabled = true;
            resources.ApplyResources(this.cbxZentaoBuild, "cbxZentaoBuild");
            this.cbxZentaoBuild.Name = "cbxZentaoBuild";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // cbxZentaoProject
            // 
            this.cbxZentaoProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxZentaoProject.FormattingEnabled = true;
            resources.ApplyResources(this.cbxZentaoProject, "cbxZentaoProject");
            this.cbxZentaoProject.Name = "cbxZentaoProject";
            this.cbxZentaoProject.SelectedIndexChanged += new System.EventHandler(this.cbxZentaoProject_SelectedIndexChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // btnRequestZentaoProduct
            // 
            resources.ApplyResources(this.btnRequestZentaoProduct, "btnRequestZentaoProduct");
            this.btnRequestZentaoProduct.Name = "btnRequestZentaoProduct";
            this.btnRequestZentaoProduct.UseVisualStyleBackColor = true;
            this.btnRequestZentaoProduct.Click += new System.EventHandler(this.btnRequestZentaoProduct_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // cbxZentaoProduct
            // 
            this.cbxZentaoProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxZentaoProduct.FormattingEnabled = true;
            resources.ApplyResources(this.cbxZentaoProduct, "cbxZentaoProduct");
            this.cbxZentaoProduct.Name = "cbxZentaoProduct";
            this.cbxZentaoProduct.SelectedIndexChanged += new System.EventHandler(this.cbxZentaoProduct_SelectedIndexChanged);
            // 
            // tabPageZentaoBug
            // 
            this.tabPageZentaoBug.Controls.Add(this.txtZentaoBug);
            resources.ApplyResources(this.tabPageZentaoBug, "tabPageZentaoBug");
            this.tabPageZentaoBug.Name = "tabPageZentaoBug";
            this.tabPageZentaoBug.UseVisualStyleBackColor = true;
            // 
            // txtZentaoBug
            // 
            this.txtZentaoBug.AcceptsReturn = true;
            resources.ApplyResources(this.txtZentaoBug, "txtZentaoBug");
            this.txtZentaoBug.Name = "txtZentaoBug";
            // 
            // tabPageZentaoStory
            // 
            this.tabPageZentaoStory.Controls.Add(this.txtZentaoStory);
            resources.ApplyResources(this.tabPageZentaoStory, "tabPageZentaoStory");
            this.tabPageZentaoStory.Name = "tabPageZentaoStory";
            this.tabPageZentaoStory.UseVisualStyleBackColor = true;
            // 
            // txtZentaoStory
            // 
            this.txtZentaoStory.AcceptsReturn = true;
            resources.ApplyResources(this.txtZentaoStory, "txtZentaoStory");
            this.txtZentaoStory.Name = "txtZentaoStory";
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnExport;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.tbExportWay);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pbExport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnTargetDirectory);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTargetDirectory);
            this.Controls.Add(this.cbxSvnRepository);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.tbExportWay.ResumeLayout(false);
            this.tabPageSvnRevision.ResumeLayout(false);
            this.tabPageSvnRevision.PerformLayout();
            this.tabPageZentaoBuild.ResumeLayout(false);
            this.tabPageZentaoBuild.PerformLayout();
            this.tabPageZentaoBug.ResumeLayout(false);
            this.tabPageZentaoBug.PerformLayout();
            this.tabPageZentaoStory.ResumeLayout(false);
            this.tabPageZentaoStory.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxSvnRepository;
        private System.Windows.Forms.TextBox txtTargetDirectory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnTargetDirectory;
        private System.Windows.Forms.FolderBrowserDialog dlgTargetDirectoryBrowser;
        private System.Windows.Forms.TextBox txtNumbers;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ProgressBar pbExport;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker exportSvnBackgroundWorker;
        private System.Windows.Forms.TabControl tbExportWay;
        private System.Windows.Forms.TabPage tabPageSvnRevision;
        private System.Windows.Forms.TabPage tabPageZentaoBuild;
        private System.Windows.Forms.ComboBox cbxZentaoProduct;
        private System.Windows.Forms.Button btnRequestZentaoProduct;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxZentaoBuild;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbxZentaoProject;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPageZentaoBug;
        private System.Windows.Forms.TextBox txtZentaoBug;
        private System.Windows.Forms.TabPage tabPageZentaoStory;
        private System.Windows.Forms.TextBox txtZentaoStory;
    }
}

