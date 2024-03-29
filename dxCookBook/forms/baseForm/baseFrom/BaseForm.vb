﻿Imports System.Collections.Specialized
Imports System.Data.SqlClient
Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Timers
Imports DevExpress.Utils.Drawing
Imports DevExpress.Utils.Helpers
Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.WinExplorer
Imports Microsoft.Office.Interop



Public Class BaseForm
    Public aTimer As Timer = New Timer() With {
            .Interval = 1000,
            .AutoReset = True,
            .Enabled = True
        }


#Region "Utils"

    Sub Print(ByVal description As String)
        ' Get Parent Method
        Dim sf As New StackFrame(1, True)
        Dim parentMethod As MethodBase = sf.GetMethod()
        Dim dots As String = New String(".", 50 - parentMethod.Name.Count())

        Debug.WriteLine($"[INFO]...{parentMethod.Name}:{dots}{description}")
    End Sub

    Sub WarningMessageBox(language As String, Optional ByVal type As String = Nothing, Optional ByVal EN As String = Nothing, Optional ByVal NL As String = Nothing)
        Print("Select Warning Message")
        Dim description As String

        ' Select Messeage type
        Select Case type
            Case "input"
                description = If(language = "ENGLISH", EN, NL)
            Case "save"
                description = If(language = "ENGLISH",
                        "Save the form before you can execute this action.",
                        "Deze handeling kunt u pas uitvoeren na het opslaan van het formulier.")
            Case "readOnly"
                description = If(language = "ENGLISH",
                        "This form Is already opened by another user, therefore it's not possible to make any modifications.",
                        "Dit formulier is al door een andere gebruiker geopend, het is daarom niet mogelijk om wijzigingen aan te brengen")
            Case Else
                description = "¯\_(-.-)_/¯"
        End Select

        ' Show warning message
        XtraMessageBox.Show(description, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

    Sub OnTimedEvent(ByVal source As Object, ByVal e As ElapsedEventArgs)
        ' Get current time
        Print($"Pulse event: {e.SignalTime}")

        ' Get new current form size
        Dim formWidth As Integer = Me.Width
        Dim formHeight As Integer = Me.Height
        Print($"Current formsize: {formWidth}, {formHeight}")
    End Sub

#End Region



#Region " Classes"

    Class BaseGridControl
        Property Gc As sslDataGrid.sslDataGrid
        Property Ds As Object
        Property Ta As Object
        Property Gv As GridView
        Private ReadOnly Dt As DataTable

        Private ReadOnly Cms As New ContextMenuStrip()
        Private WithEvents CmsItemOpen As New ToolStripMenuItem()
        Private WithEvents CmsItemNew As New ToolStripMenuItem()
        Private WithEvents CmsItemSort As New ToolStripMenuItem()
        Private WithEvents CmsItemDelete As New ToolStripMenuItem()

        Sub New(gc As sslDataGrid.sslDataGrid, ds As Object, ta As Object)
            Me.Gc = gc
            Me.Ds = ds
            Me.Ta = ta
            Gv = gc.MainView

            ' Fill Table
            Me.Ta.Fill(ds)
            Dt = Me.Ds
            Me.Gc.DataSource = Dt

            ' Grid Control Settings
            Gv.OptionsView.ColumnAutoWidth = False
            Gv.HorzScrollVisibility = True
            Gv.OptionsBehavior.Editable = False
            Gv.OptionsView.ShowGroupPanel = False
            Gv.OptionsFind.AllowFindPanel = True
            Gv.OptionsFind.AlwaysVisible = True
            Gv.OptionsView.ShowFooter = True

            ' ContextMenuStrip Settings
            CmsItemOpen.Name = "Open"
            CmsItemOpen.Text = "Open"
            CmsItemNew.Name = "New"
            CmsItemNew.Text = "New"
            CmsItemDelete.Name = "Delete"
            CmsItemDelete.Text = "Delete"
            Cms.Items.Add(CmsItemOpen)
            Cms.Items.Add(CmsItemNew)
            Cms.Items.Add(CmsItemDelete)

            ' Handlers
            AddHandler gc.DoubleClick, AddressOf CmsItemOpen_Click
            AddHandler Gv.Click, AddressOf ShowContextMenuStrip_RightClick
            AddHandler CmsItemOpen.Click, AddressOf CmsItemOpen_Click
            AddHandler CmsItemNew.Click, AddressOf CmsItemNew_Click
            AddHandler CmsItemDelete.Click, AddressOf CmsItemDelete_Click
        End Sub



#Region "Handlers"

        Private Sub ShowContextMenuStrip_RightClick(sender As Object, e As DevExpress.Utils.DXMouseEventArgs)
            Print("Right mouse click handler, show popup-menu")
            Dim dataRow As DataRow = Gv.GetFocusedDataRow()
            Try
                If e.Button = MouseButtons.Right And dataRow("konummer") > 0 Then Cms.Show(MousePosition)
            Catch ex As NullReferenceException
                Print($"No row was selected: {ex.Message}")
            End Try
        End Sub

        Private Sub CmsItemOpen_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim dataRow As DataRow = Gv.GetFocusedDataRow()
            MsgBox($"Selected row id: {dataRow("Konummer")}")
            ' GridView3_DoubleClick
        End Sub

        Private Sub CmsItemNew_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim dataRow As DataRow = Gv.GetFocusedDataRow()
            MsgBox($"New Row")
            ' BtnNewCostRevenue_Click
        End Sub

        Private Sub CmsItemSort_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim dataRow As DataRow = Gv.GetFocusedDataRow()
            MsgBox($"Sort")
        End Sub

        Private Sub CmsItemDelete_Click(sender As Object, e As EventArgs)
            ' Get Selected Row
            Dim dataRow As DataRow = Gv.GetFocusedDataRow()

            '' Exectue StoredProcedure
            'Dim con As New SqlConnection(My.Settings.conSsl)
            'Dim cmd As New SqlCommand("spDeleteCostRevenue", con) With {
            '    .CommandType = CommandType.StoredProcedure
            '}
            'cmd.Parameters.AddWithValue("@CRCODE", dataRow("Konummer"))
            'con.Open()
            'cmd.ExecuteNonQuery()

            '' Check if Execute Query was successful
            'If cmd.ExecuteNonQuery() > 0 Then
            '    dataRow.Delete()
            'Else
            '    XtraMessageBox.Show("Delete row was not successful", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'End If
            'con.Close()
        End Sub

#End Region ' Handlers



#Region "Methods"

        Private Sub Print(ByVal description As String)
            ' Get parent method
            Dim sf As New StackFrame(1, True)
            Dim parentMethod As MethodBase = sf.GetMethod()
            Dim dots As String = New String(".", 50 - parentMethod.Name.Count())
            ' Print
            Debug.WriteLine($"[INFO]...{parentMethod.Name}:{dots}{description}")
        End Sub

#End Region ' Methods



#Region "Notes"

        '---------------------------------------
        'Initialize Class Example
        '---------------------------------------
        'BaseGridControlExmaple = New BaseGridControl(
        '   SslDataGrid1,
        '   New dsAlphaTable.AlphaTableDataTable(),
        '   New dsAlphaTableTableAdapters.AlphaTableTableAdapter()
        '   )

        '---------------------------------------
        ' Modify GridView Settings in Child Class
        '---------------------------------------
        'BaseGridControlExmaple. Gv.OptionsView.ColumnAutoWidth = False
        'BaseGridControlExmaple.Gv.HorzScrollVisibility = True
        'BaseGridControlExmaple.Gv.OptionsBehavior.Editable = False
        'BaseGridControlExmaple.Gv.OptionsView.ShowGroupPanel = False
        'BaseGridControlExmaple.Gv.OptionsFind.AllowFindPanel = True
        'BaseGridControlExmapleGv.OptionsFind.AlwaysVisible = True
        'BaseGridControlExmaple.Gv.OptionsView.ShowFooter = True

#End Region
    End Class


    Class FileManagerView
        Implements IFileSystemNavigationSupports
        Property Id As Integer
        Property GReadOnly As Boolean = Globals.gReadOnly
        Private ReadOnly GPath As String = Globals.gPath
        Private ReadOnly GLanguage As String = Globals.gLanguage
        Private IconSize As WinExplorerViewStyle = WinExplorerViewStyle.Medium
        Private FormDir As String
        Private Gc As sslDataGrid.sslDataGrid
        Private WithEvents Wv As WinExplorerView = New WinExplorerView()
        Private Cms As ContextMenuStrip = New ContextMenuStrip()
        Private WithEvents CmsItemRename As New ToolStripMenuItem()
        Private WithEvents CmsItemSmall As New ToolStripMenuItem()
        Private WithEvents CmsItemMedium As New ToolStripMenuItem()
        Private WithEvents CmsItemLarge As New ToolStripMenuItem()
        Private WithEvents CmsItemXL As New ToolStripMenuItem()
        Private WithEvents CmsItemCopy As New ToolStripMenuItem()
        Private WithEvents CmsItemOpen As New ToolStripMenuItem()
        Private WithEvents CmsItemDelete As New ToolStripMenuItem()
        Private WithEvents CmsItemEmail As New ToolStripMenuItem()
        Private WithEvents CmsItemMove As New ToolStripMenuItem()

        Sub New(ByRef pFileSystem As sslDataGrid.sslDataGrid, ByRef pReadOnly As Boolean, Optional ByVal pID As Integer = Nothing)
            Gc = pFileSystem
            Id = pID
            GReadOnly = pReadOnly
            FormDir = Path.Combine(GPath, Id)

            InitializeWinExplorerView()
            InitializeContextMenuStrip()

            AddHandler Me.Gc.DragEnter, AddressOf Gc_DragEnter
            AddHandler Me.Gc.DragDrop, AddressOf Gc_DragDrop
            AddHandler Me.Wv.Click, AddressOf WinExplorerView_RightClick
            AddHandler Me.Wv.DoubleClick, AddressOf CmsItemOpen_Click
            AddHandler Me.Wv.KeyDown, AddressOf Handler_KeyDown
            AddHandler Me.CmsItemRename.Click, AddressOf CmsItemRename_Click
            AddHandler Me.CmsItemSmall.Click, AddressOf CmsItemSmall_Click
            AddHandler Me.CmsItemMedium.Click, AddressOf CmsItemMedium_Click
            AddHandler Me.CmsItemLarge.Click, AddressOf CmsItemLarge_Click
            AddHandler Me.CmsItemXL.Click, AddressOf CmsItemXL_Click
            AddHandler Me.CmsItemCopy.Click, AddressOf CmsItemCopy_Click
            AddHandler Me.CmsItemOpen.Click, AddressOf CmsItemOpen_Click
            AddHandler Me.CmsItemDelete.Click, AddressOf CmsItemDelete_Click
            AddHandler Me.CmsItemEmail.Click, AddressOf CmsItemEmail_Click
            AddHandler Me.CmsItemMove.Click, AddressOf CmsItemMove_Click
        End Sub




#Region "Initializers"

        Private Sub InitializeWinExplorerView()
            Print($"Declare columns and settings")

            ' Declare Columns
            Dim colName As New GridColumn() With {
                .Caption = "columnName",
                .Visible = True,
                .VisibleIndex = 0,
                .FieldName = "Name",
                .Name = "columnName"
            }
            Dim colPath As New GridColumn() With {
                .Caption = "columnPath",
                .Visible = True,
                .VisibleIndex = 0,
                .FieldName = "Path",
                .Name = "columnPath"
            }
            Dim colCheck As New GridColumn() With {
                .Caption = "columnCheck",
                .Visible = True,
                .VisibleIndex = 0,
                .FieldName = "IsCheck",
                .Name = "columnCheck"
            }
            Dim colGroup As New GridColumn() With {
                .Caption = "columnGroup",
                .Visible = True,
                .VisibleIndex = 0,
                .FieldName = "Group",
                .Name = "columnGroup"
            }
            Dim colImage As New GridColumn() With {
                .Caption = "columnImage",
                .Visible = True,
                .VisibleIndex = 0,
                .FieldName = "Image",
                .Name = "columnImage"
            }

            ' WinExplorerView settings
            Wv.Columns.Add(colName)
            Wv.Columns.Add(colPath)
            Wv.Columns.Add(colCheck)
            Wv.Columns.Add(colGroup)
            Wv.Columns.Add(colImage)

            Wv.ColumnSet.CheckBoxColumn = colCheck
            Wv.ColumnSet.DescriptionColumn = colPath
            Wv.ColumnSet.ExtraLargeImageColumn = colImage
            Wv.ColumnSet.LargeImageColumn = colImage
            Wv.ColumnSet.MediumImageColumn = colImage
            Wv.ColumnSet.SmallImageColumn = colImage
            Wv.ColumnSet.TextColumn = colName

            Wv.OptionsSelection.AllowMarqueeSelection = True
            Wv.OptionsSelection.ItemSelectionMode = IconItemSelectionMode.Click
            Wv.OptionsSelection.MultiSelect = True

            Wv.OptionsView.ImageLayoutMode = ImageLayoutMode.Stretch
            Wv.OptionsView.Style = IconSize
            Wv.OptionsView.ShowViewCaption = True

            Wv.OptionsFind.AlwaysVisible = True

            ' GridControl settings
            Gc.MainView = Wv
            Gc.AllowDrop = Not GReadOnly

            If Not IsNothing(Id) And Directory.Exists(FormDir) Then
                Print($"Show Files from Directory")
                Gc.DataSource = FileSystemHelper.GetFileSystemEntries(FormDir, GetItemSizeType(Wv.OptionsView.Style), GetItemSize(Wv.OptionsView.Style))
            End If
        End Sub

        Private Sub InitializeContextMenuStrip()
            Print($"Build & add menu items")

            ' Menu Items Settings
            CmsItemRename.Name = "Rename"
            CmsItemRename.Text = "Rename"

            CmsItemSmall.Name = "Small"
            CmsItemSmall.Text = "Small"

            CmsItemMedium.Name = "Medium"
            CmsItemMedium.Text = "Medium"

            CmsItemLarge.Name = "Large"
            CmsItemLarge.Text = "Large"

            CmsItemXL.Name = "ExtraLarge"
            CmsItemXL.Text = "Extra Large"

            CmsItemCopy.Name = "Copy"
            CmsItemCopy.Text = "Copy"

            CmsItemDelete.Name = "Delete"
            CmsItemDelete.Text = "Delete"

            CmsItemOpen.Name = "Open"
            CmsItemOpen.Text = "Open"

            CmsItemEmail.Name = "Mail"
            CmsItemEmail.Text = "Mail"

            CmsItemMove.Name = "Move"
            CmsItemMove.Text = "Move"

            ' Add Menu items to ContextMenuStrip
            Cms.Items.Add(CmsItemOpen)
            Cms.Items.Add(CmsItemSmall)
            Cms.Items.Add(CmsItemMedium)
            Cms.Items.Add(CmsItemLarge)
            Cms.Items.Add(CmsItemXL)
            Cms.Items.Add(CmsItemDelete)
            Cms.Items.Add(CmsItemMove)
            Cms.Items.Add(CmsItemEmail)
            Cms.Items.Add(CmsItemCopy)
            Cms.Items.Add(CmsItemRename)
        End Sub

#End Region



#Region "Handlers"

        Private Sub Gc_DragDrop(sender As Object, e As DragEventArgs)
            Print($"Drag and drop handler")

            If IsNothing(Id) Or Id <> 0 Then

                ' Set for Directory
                FormDir = Path.Combine(GPath, Id)

                ' Create directory
                If Not Directory.Exists(FormDir) Then Directory.CreateDirectory(FormDir)

                ' Drop File into WinExplorerView
                Try
                    If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                        ' List of Dragged Files
                        Dim draggedFiles As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())

                        ' Copy file to form directory
                        For Each i As String In draggedFiles

                            File.Copy(i, Path.Combine(FormDir, Path.GetFileName(i)), True)
                        Next

                    ElseIf e.Data.GetDataPresent("FileGroupDescriptor") Then
                        DropEmail(sender, e)
                    End If

                Catch ex As DirectoryNotFoundException
                    Print($"¯\_(^,^)_/¯ : Can't find Directory" & vbCrLf & ex.Message)
                Catch ex As NullReferenceException
                    Print($"¯\_('v')_/¯ : I don't get this filetype" & vbCrLf & ex.Message)
                Catch ex As Exception
                    Print($"¯\_(-.-)_/¯ : I don't even know what this is" & vbCrLf & ex.Message)
                End Try

                ' Update View
                ClearView()
                InitializeWinExplorerView()
            Else
                WarningMessageBox("save")
            End If
        End Sub

        Private Sub DropEmail(sender As Object, e As DragEventArgs)
            Print($"Enable Email Drag & Drop")

            Dim mem_stream As MemoryStream = e.Data.GetData("FileGroupDescriptor")
            Dim bytes(mem_stream.Length - 1) As Byte

            mem_stream.Read(bytes, 0, mem_stream.Length)
            mem_stream.Close()
            Dim fnames$() = Nothing, sw As Boolean = False
            For f1 As Integer = 76 To (bytes.Length - 1)
                If bytes(f1) = 0 Then
                    sw = False
                Else
                    If Not sw Then
                        sw = True
                        If IsNothing(fnames) Then ReDim fnames(0) Else ReDim Preserve fnames(fnames.Length)
                        fnames(fnames.Length - 1) = ""
                    End If
                    fnames(fnames.Length - 1) &= Chr(bytes(f1))
                End If
            Next f1

            Dim sType As String = fnames(0).ToString()

            If Strings.Right(sType, 4).ToUpper() = ".MSG" Then
                'supports a drop of a Outlook message

                ' Int Outlook Application
                Dim objOL As New Microsoft.Office.Interop.Outlook.Application

                'Dim objMI As Object - if you want to do late-binding
                Dim objMI As Microsoft.Office.Interop.Outlook.MailItem

                For Each objMI In objOL.ActiveExplorer.Selection()
                    'hardcode a destination path for testing
                    Dim dt As String = objMI.ReceivedTime.Year & "-" & objMI.ReceivedTime.Month & "-" & objMI.ReceivedTime.Day & "-" & objMI.ReceivedTime.Hour & "-" _
                    & objMI.ReceivedTime.Minute & "-" & objMI.ReceivedTime.Second

                    Dim strFile As String =
                            IO.Path.Combine(FormDir, (FnEmailSubject(objMI.Subject) + "_" + dt + ".msg").Replace(":", ""))
                    objMI.SaveAs(strFile)
                Next
            Else
                Dim theFile As String = FormDir & DateTime.Now.ToString("yyyyMMdd_HHmmss") & "_" & fnames(0).ToString()

                ' get the actual raw file into memory
                Dim ms As MemoryStream = CType(e.Data.GetData("FileContents"), MemoryStream)

                ' allocate enough bytes to hold the raw data
                Dim fileBytes(ms.Length) As Byte

                ' set starting position at first byte And read in the raw data
                ms.Position = 0
                ms.Read(fileBytes, 0, ms.Length)

                ' create a file and save the raw zip file to it
                Dim fs As FileStream = New FileStream(theFile, FileMode.Create)
                fs.Write(fileBytes, 0, fileBytes.Length)

                ' close the file
                fs.Close()
            End If
        End Sub

        Private Sub Gc_DragEnter(sender As Object, e As DragEventArgs)
            If Not GReadOnly Then
                Print($"Drag Enter handler")
                e.Effect = DragDropEffects.All
            Else
                WarningMessageBox("readOnly")
            End If
        End Sub

        Private Sub CmsItemDelete_Click(sender As Object, e As EventArgs)
            If Not GReadOnly Then
                Print($"Delete selected files handler")
                ' Get selected files from WinExplorerView
                Dim selectedRows() As Integer = Wv.GetSelectedRows()

                ' Iterate selected files
                For Each i As Integer In selectedRows
                    Dim fileEntry As FileSystemEntry = CType(Wv.GetRow(i), FileSystemEntry)
                    Try
                        ' Delete File
                        File.Delete(fileEntry.Path)

                        ' Refesh View
                        ClearView()
                        InitializeWinExplorerView()

                    Catch ex As Exception
                        Print($"User can't delete file: {ex}")
                    End Try
                Next
            Else
                WarningMessageBox("readOnly")
            End If
        End Sub

        Private Sub Handler_KeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Delete Then
                If Not GReadOnly Then
                    Print("Delete file with KeyDown handler")

                    ' Get selected files from WinExplorerView
                    Dim selectedRows() As Integer = Wv.GetSelectedRows()

                    ' Iterate selected files
                    For Each i As Integer In selectedRows
                        Dim fileEntry As FileSystemEntry = CType(Wv.GetRow(i), FileSystemEntry)
                        Try
                            ' Delete File
                            File.Delete(fileEntry.Path)

                            ' Refesh View
                            ClearView()
                            InitializeWinExplorerView()

                        Catch ex As Exception
                            Print($"User can't delete file: {ex}")
                        End Try
                    Next
                Else
                    WarningMessageBox("readOnly")
                End If
            End If
        End Sub

        Private Sub CmsItemOpen_Click(sender As Object, e As EventArgs)
            Print($"Open selected files handler")

            ' Get selected files from WinExplorerView
            Dim selectedRows() As Integer = Wv.GetSelectedRows()

            ' Iterate selected files
            For Each i As Integer In selectedRows
                Dim fileEntry As FileSystemEntry = CType(Wv.GetRow(i), FileSystemEntry)
                Try
                    ' Open File
                    fileEntry.DoAction(Me)
                Catch ex As ArgumentNullException
                    Print($"User can't access folder: {ex}")
                End Try
            Next
        End Sub

        Private Sub CmsItemCopy_Click(sender As Object, e As EventArgs)
            Print($"Copy files to Clipboard handler")

            ' Get selected files from WinExplorerView
            Dim selectedRows() As Integer = Wv.GetSelectedRows()

            ' List all selected file paths
            Dim sc As StringCollection = New StringCollection()
            For Each i As Integer In selectedRows
                Dim fileEntry As FileSystemEntry = CType(Wv.GetRow(i), FileSystemEntry)
                sc.Add(fileEntry.Path)
            Next

            ' Add filepaths to clipboard
            Dim dataObj As New DataObject()
            dataObj.SetFileDropList(sc)
            Clipboard.SetDataObject(dataObj, True)
        End Sub

        Private Sub CmsItemXL_Click(sender As Object, e As EventArgs)
            Print($"Set Icon XL size handler")
            IconSize = WinExplorerViewStyle.ExtraLarge
            Wv.OptionsView.Style = IconSize
            Wv.OptionsViewStyles.ExtraLarge.ImageSize = New Size(256, 256)
        End Sub

        Private Sub CmsItemLarge_Click(sender As Object, e As EventArgs)
            Print($"Set Icon Large size handler")
            IconSize = WinExplorerViewStyle.Large
            Wv.OptionsView.Style = IconSize
            Wv.OptionsViewStyles.ExtraLarge.ImageSize = New Size(96, 96)
        End Sub

        Private Sub CmsItemMedium_Click(sender As Object, e As EventArgs)
            Print($"Set Icon Medium size handler")
            IconSize = WinExplorerViewStyle.Medium
            Wv.OptionsView.Style = IconSize
            Wv.OptionsViewStyles.ExtraLarge.ImageSize = New Size(32, 32)
        End Sub

        Private Sub CmsItemSmall_Click(sender As Object, e As EventArgs)
            Print($"Set Icon Small size handler")
            IconSize = WinExplorerViewStyle.Small
            Wv.OptionsView.Style = IconSize
            Wv.OptionsViewStyles.ExtraLarge.ImageSize = New Size(16, 16)
        End Sub

        Private Sub CmsItemRename_Click(sender As Object, e As EventArgs)
            If Not GReadOnly Then
                Print("Rename file handler")
                Dim fileIndex As Integer = Wv.FocusedRowHandle
                Dim fileEntry As FileSystemEntry = CType(Wv.GetRow(fileIndex), FileSystemEntry)
                Dim filePath As String = fileEntry.Path
                Dim fileName As String = fileEntry.Name
                Dim fileExtension As String = Path.GetExtension(filePath)

                Print($"GetRow: {Wv.GetRow(fileIndex)}")
                Print($"filePath: {filePath}")
                Print($"fileName: {fileName}")
                Print($"fileExtension: {fileExtension}")

                If File.Exists(filePath) Then
                    Try
                        Dim newName As String = XtraInputBox.Show($"Rename {fileName}", Application.CompanyName, "")
                        My.Computer.FileSystem.RenameFile(filePath, $"{newName}{fileExtension}")
                    Catch ex As IOException
                        WarningMessageBox("The file name already exists.", "De opgegeven bestandsnaam bestaat al.")
                    Finally
                        ClearView()
                        InitializeWinExplorerView()
                    End Try
                Else
                    Print($"File '{filePath}' does not exists :(")
                End If
            Else
                WarningMessageBox("readOnly")
            End If
        End Sub

        Private Sub CmsItemEmail_Click(sender As Object, e As EventArgs)
            Print($"Email files handler")

            ' Get selected files from WinExplorerView
            Dim selectedRows() As Integer = Wv.GetSelectedRows()

            ' Build Email
            Dim outlookApp As New Outlook.Application()
            Dim mailItem As Outlook.MailItem = outlookApp.CreateItem(Outlook.OlItemType.olMailItem)
            mailItem.Subject = If(GLanguage = "ENGLISH",
                $"Attachments - Filenumber: {Id}",
                $"Bestanden - Dossier: {Id} - ")

            ' Iterate selected files
            For Each i As Integer In selectedRows
                Dim fileEntry As FileSystemEntry = CType(Wv.GetRow(i), FileSystemEntry)

                ' Add file to mail
                mailItem.Attachments.Add(fileEntry.Path)
                mailItem.Display(False)
            Next
        End Sub

        Private Sub CmsItemMove_Click(sender As Object, e As EventArgs)

            If Not GReadOnly Then
                Print($"Move file('s) to a different directory")
                Try
                    ' User Inputs target directory to move files to
                    Dim targetFolder As Integer = XtraInputBox.Show($"Copy files to", Application.CompanyName, "")
                    If Directory.Exists(Path.Combine(GPath, targetFolder)) Then
                        ' Copy Files
                        CopySelectedFiles(targetFolder)
                    Else
                        ' Create Directory & Copy Files
                        Directory.CreateDirectory(Path.Combine(GPath, targetFolder))
                        CopySelectedFiles(targetFolder)
                    End If
                Catch ex As InvalidCastException
                    ' User has not input any value, or did not input an Integer
                    Console.WriteLine(ex)
                End Try
            Else
                WarningMessageBox("readOnly")
            End If
        End Sub

        Private Sub WinExplorerView_RightClick(sender As Object, e As DevExpress.Utils.DXMouseEventArgs)
            Print("Right mouse click handler, show popup-menu ")
            If e.Button = MouseButtons.Right Then Cms.Show(MousePosition)
        End Sub

#End Region



#Region "Methods"

        Public Function FnEmailSubject(ByVal subject As String) As String
            Print("")
            If subject = Nothing Then
                subject = "EMPTY SUBJECT"
            Else
                subject = subject.Trim
                If subject <> "" Then
                    subject = Regex.Replace(subject, ",|\.|\\|\/|\[|\]|\'|\%|\:|\;|\t|\#|\*|\<|\>|\" & Chr(34) & "|\" & Chr(47), "_")
                    subject = subject.Replace("|", "_")
                End If
            End If
            Return subject
        End Function

        Sub CopySelectedFiles(ByVal targetFolder As String)
            Print($"Copy files to {targetFolder} directory")

            ' Get selected files from WinExplorerView
            Dim selectedRows() As Integer = Wv.GetSelectedRows()

            ' Iterate selected files
            For Each i As Integer In selectedRows
                Dim fileEntry As FileSystemEntry = CType(Wv.GetRow(i), FileSystemEntry)
                Dim filePath As String = fileEntry.Path
                Dim fileName As String = fileEntry.Name
                Dim fileExtension As String = Path.GetExtension(filePath)

                ' Copy non-folder file to target folder
                If Not Wv.IsGroupRow(i) Then
                    Try
                        File.Copy(filePath, Path.Combine(GPath, targetFolder, fileName & fileExtension), True)
                    Catch ex As IOException
                        ' current file is a folder
                        Print(ex.ToString)
                    End Try
                End If
            Next i
        End Sub

        Sub ClearView()
            Print("Clear WinExplorerView MainView")
            Gc.BeginUpdate()
            Try
                Wv.Columns.Clear()
                Gc.DataSource = Nothing
            Finally
                Gc.EndUpdate()
            End Try
        End Sub

        Private Function GetItemSizeType(ByVal viewStyle As WinExplorerViewStyle) As IconSizeType
            Print("Set Icon Size Type")
            Select Case viewStyle
                Case WinExplorerViewStyle.Large, WinExplorerViewStyle.ExtraLarge
                    Return IconSizeType.ExtraLarge
                Case WinExplorerViewStyle.List, WinExplorerViewStyle.Small
                    Return IconSizeType.Small
                Case WinExplorerViewStyle.Tiles, WinExplorerViewStyle.Medium, WinExplorerViewStyle.Content
                    Return IconSizeType.Large
                Case Else
                    Return IconSizeType.ExtraLarge
            End Select
        End Function

        Private Function GetItemSize(ByVal viewStyle As WinExplorerViewStyle) As Size
            Print("Set icon item size")
            Select Case viewStyle
                Case WinExplorerViewStyle.ExtraLarge
                    Return New Size(256, 256)
                Case WinExplorerViewStyle.Large
                    Return New Size(96, 96)
                Case WinExplorerViewStyle.Content
                    Return New Size(32, 32)
                Case WinExplorerViewStyle.Small
                    Return New Size(16, 16)
                Case Else
                    Return New Size(96, 96)
            End Select
        End Function

        Private Sub Print(ByVal description As String)
            ' Get Parent Method
            Dim sf As New StackFrame(1, True)
            Dim parentMethod As MethodBase = sf.GetMethod()
            Dim dots As String = New String(".", 50 - parentMethod.Name.Count())

            Debug.WriteLine($"[INFO]...{parentMethod.Name}:{dots}{description}")
        End Sub

        Private Sub WarningMessageBox(Optional ByVal type As String = Nothing, Optional ByVal EN As String = Nothing, Optional ByVal NL As String = Nothing)
            Print("Select Warning Message")
            Dim description As String

            ' Select Messeage type
            Select Case type
                Case "input"
                    description = If(GLanguage = "ENGLISH", EN, NL)
                Case "save"
                    description = If(GLanguage = "ENGLISH",
                        "Save the form before you can execute this action.",
                        "Deze handeling kunt u pas uitvoeren na het opslaan van het formulier.")
                Case "readOnly"
                    description = If(GLanguage = "ENGLISH",
                        "This form Is already opened by another user, therefore it's not possible to make any modifications.",
                        "Dit formulier is al door een andere gebruiker geopend, het is daarom niet mogelijk om wijzigingen aan te brengen")
                Case Else
                    description = "¯\_(-.-)_/¯"
            End Select

            ' Show warning message
            XtraMessageBox.Show(description, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Sub

#End Region



#Region "Implements"
        Public ReadOnly Property CurrentPath As String Implements IFileSystemNavigationSupports.CurrentPath
            Get
                Try
                    Throw New NotImplementedException()
                Catch ex As NotImplementedException
                    Print($"User can't access folder: {ex}")
                End Try
                Return Nothing
            End Get
        End Property

        Public Sub UpdatePath(path As String) Implements IFileSystemNavigationSupports.UpdatePath
            Throw New NotImplementedException()
        End Sub

#End Region



#Region "Guide"

        '-----------------------------------------------
        '           Setup File Manager View
        '-----------------------------------------------

        ' 1. Copy this Sub Class to the Parent From (BaseFrom)
        ' 2. In the Form Design, place a sslDataGrid
        ' 3. In the Child From declare Class Variable

        '   Private fileManagerView1 As FileManagerView

        '4. Initialize Class Variable in the constructor.

        '   fileManagerView1 = New FileManagerView(Me.SslDataGrid2, gReadOnly, Globals.ID)

        ' The FileManagerView contructer takes 3 parameters.
        '   - First is the name of the SslDataGrid
        '   - Second is a Boolean value for giving permission to the user, to modify files inside the FileMangerView
        '   - Third is the form id, this value does not need to be initiated in the construction stage.

        '5. Create a Save button handler if you don't already have one and place an id setter.

        '   fileManagerView1.Id = Globals.ID

        '   When the form id is available at "save new from" you can set it for the File Manager View


#End Region

    End Class

#End Region

End Class