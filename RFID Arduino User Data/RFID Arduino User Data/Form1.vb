Imports Mysql.Data.MySqlClient

Public Class Form1

    Dim Connection As New MySqlConnection("server=localhost; user=root; password=; database=rfid_user_data")
    Dim MySQLCMD As New MySqlCommand
    Dim MySQLDA As New MySqlDataAdapter
    Dim DT As New DataTable
    Dim Table_Name As String = "rfid_user_data_table"
    Dim Data As Integer

    Dim LoadImagesStr As Boolean = False
    Dim IDRam As String
    Dim IMG_FileNameInput As String
    Dim StatusInput As String = "Save"
    Dim SqlCmdSearchstr As String

    Public Shared StrSerialIn As String
    Dim GetID As Boolean = False
    Dim ViewUserData As Boolean = False


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BtnConnection.Click
        PictureBoxSelect.Top = BtnConnection.Top
        PanelRegistrationandUserData.Visible = False
        PanelUserData.Visible = False
        PanelConnection.Visible = True
    End Sub

    Private Sub BtnUserData_Click(sender As Object, e As EventArgs) Handles BtnUserData.Click
        If TimerSerialIn.Enabled = False Then
            MsgBox("Failed to open User Data !!!" & vbCr & "Click the connection menu then click the Connection button.", MsgBoxStyle.Information, "Information")
            Return
        Else
            StrSerialIn = ""
            ViewUserData = True
            PictureBoxSelect.Top = BtnUserData.Top
            PanelRegistrationandUserData.Visible = False
            PanelConnection.Visible = False
            PanelUserData.Visible = True
        End If
    End Sub

    Private Sub BtnRegistration_Click(sender As Object, e As EventArgs) Handles BtnRegistration.Click
        StrSerialIn = ""
        ViewUserData = False
        PictureBoxSelect.Top = BtnRegistration.Top
        PanelUserData.Visible = False
        PanelConnection.Visible = False
        PanelRegistrationandUserData.Visible = True
        ShowData()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs)

    End Sub


    Private Sub PanelConnection_Paint(sender As Object, e As PaintEventArgs) Handles PanelConnection.Paint
        e.Graphics.DrawRectangle(New Pen(Color.LightGray, 2), PanelConnection.ClientRectangle)
    End Sub

    Private Sub PanelConnection_Resize(sender As Object, e As EventArgs) Handles PanelConnection.Resize
        PanelConnection.Invalidate()
    End Sub

    Private Sub BtnScanPort_Click(sender As Object, e As EventArgs) Handles BtnScanPort.Click
        ComboBoxPort.Items.Clear()
        Dim myPort As Array
        Dim i As Integer
        myPort = IO.Ports.SerialPort.GetPortNames()
        ComboBoxPort.Items.AddRange(myPort)
        i = ComboBoxPort.Items.Count
        i = i - i
        Try
            ComboBoxPort.SelectedIndex = i
        Catch ex As Exception
            MsgBox("Com port not detected", MsgBoxStyle.Critical, "Error Message")
            ComboBoxPort.Text = ""
            ComboBoxPort.Items.Clear()
            Return
        End Try
        ComboBoxPort.DroppedDown = True
    End Sub

    Private Sub BtnScanPort_MouseHover(sender As Object, e As EventArgs) Handles BtnScanPort.MouseHover
        BtnScanPort.ForeColor = Color.White

    End Sub

    Private Sub BtnScanPort_MouseLeave(sender As Object, e As EventArgs) Handles BtnScanPort.MouseLeave
        BtnScanPort.ForeColor = Color.FromArgb(6, 71, 165)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.CenterToScreen()
        PanelRegistrationandUserData.Visible = False
        PanelUserData.Visible = False
        PanelConnection.Visible = True
        PanelReadingTagProcess.Visible = False
        ComboBoxBaudRate.SelectedIndex = 3
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        GroupBoxImage.Location = New Point((PanelUserData.Width / 2) - (GroupBoxImage.Width / 2), GroupBoxImage.Top)
        PanelReadingTagProcess.Location = New Point((PanelRegistrationandUserData.Width / 2) - (PanelReadingTagProcess.Width / 2), 106)
    End Sub

    Private Sub PanelUserData_Paint(sender As Object, e As PaintEventArgs) Handles PanelUserData.Paint
        e.Graphics.DrawRectangle(New Pen(Color.LightGray, 2), PanelConnection.ClientRectangle)
    End Sub

    Private Sub PanelUserData_Resize(sender As Object, e As EventArgs) Handles PanelUserData.Resize
        PanelUserData.Invalidate()
    End Sub

    Private Sub PanelRegistrationandUserData_Paint(sender As Object, e As PaintEventArgs) Handles PanelRegistrationandUserData.Paint
        e.Graphics.DrawRectangle(New Pen(Color.LightGray, 2), PanelConnection.ClientRectangle)
    End Sub

    Private Sub PanelRegistrationandUserData_Resize(sender As Object, e As EventArgs) Handles PanelRegistrationandUserData.Resize
        PanelRegistrationandUserData.Invalidate()
    End Sub

    Private Sub ShowData()
        Try
            Connection.Open()
        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            If LoadImagesStr = False Then
                MySQLCMD.CommandType = CommandType.Text
                MySQLCMD.CommandText = "SELECT Name, ID, Address, City, Country FROM " & Table_Name & " ORDER BY Name"
                MySQLDA = New MySqlDataAdapter(MySQLCMD.CommandText, Connection)
                DT = New DataTable
                Data = MySQLDA.Fill(DT)
                If Data > 0 Then
                    DataGridView1.DataSource = Nothing
                    DataGridView1.DataSource = DT
                    DataGridView1.Columns(2).DefaultCellStyle.Format = "c"
                    DataGridView1.DefaultCellStyle.ForeColor = Color.Black
                    DataGridView1.ClearSelection()
                Else
                    DataGridView1.DataSource = DT
                End If
            Else
                MySQLCMD.CommandType = CommandType.Text
                MySQLCMD.CommandText = "SELECT Images FROM " & Table_Name & " WHERE ID LIKE '" & IDRam & "'"
                MYSQLDA = New MySqlDataAdapter(MySQLCMD.CommandText, Connection)
                DT = New DataTable
                Data = MySQLDA.Fill(DT)
                If Data > 0 Then
                    Dim ImgArray() As Byte = DT.Rows(0).Item("Images")
                    Dim lmgStr As New System.IO.MemoryStream(ImgArray)
                    PictureBoxImagePreview.Image = Image.FromStream(lmgStr)
                    PictureBoxImagePreview.SizeMode = PictureBoxSizeMode.Zoom
                    lmgStr.Close()
                End If
                LoadImagesStr = False
            End If
        Catch ex As Exception
            MsgBox("Failed to load Database !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
            Return
        End Try

        DT = Nothing
        Connection.Close()
    End Sub

    Private Sub ShowDataUser()
        Try
            Connection.Open()
        Catch es As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            MySQLCMD.CommandType = CommandType.Text
            MySQLCMD.CommandText = "SELECT * FROM " & Table_Name & " WHERE ID LIKE '" & LabelID.Text.Substring(5, LabelID.Text.Length - 5) & "'"
            MYSQLDA = New MySqlDataAdapter(MySQLCMD.CommandText, Connection)
            DT = New DataTable
            Data = MySQLDA.Fill(DT)
            If Data > 0 Then
                Dim ImgArray() As Byte = DT.Rows(0).Item("Images")
                Dim lmgStr As New System.IO.MemoryStream(ImgArray)
                PictureBoxUserImage.Image = Image.FromStream(lmgStr)
                lmgStr.Close()

                LabelID.Text = "ID : " & DT.Rows(0).Item("ID")
                LabelName.Text = DT.Rows(0).Item("Name")
                LabelAddress.Text = DT.Rows(0).Item("Address")
                LabelCity.Text = DT.Rows(0).Item("City")
                LabelCountry.Text = DT.Rows(0).Item("Country")
            Else
                MsgBox("ID not found !!!" & vbCr & "Please register your ID.", MsgBoxStyle.Information, "Information Message")
            End If
        Catch ex As Exception
            MsgBox("failed to load Database !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
            Return
        End Try

        DT = Nothing
        Connection.Close()
    End Sub

    Private Sub ClearInputUpdateData()
        TextBoxName.Text = ""
        LabelGetID.Text = "_______"
        TextBoxAddress.Text = ""
        TextBoxCity.Text = ""
        TextBoxCountry.Text = ""
        PictureBoxImageInput.Image = My.Resources.click_to_browse_Image
    End Sub

    Private Sub BtnConnect_Click(sender As Object, e As EventArgs) Handles BtnConnect.Click
        If BtnConnect.Text = "Connect" Then
            SerialPort1.BaudRate = ComboBoxBaudRate.SelectedItem
            SerialPort1.PortName = ComboBoxPort.SelectedItem
            Try
                SerialPort1.Open()
                TimerSerialIn.Start()
                BtnConnect.Text = "Disconnect"
                PictureBoxStatusConnect.Image = My.Resources.Connected
            Catch ex As Exception
                MsgBox("Failed to Connect !!!" & vbCr & "Arduino is not detected.", MsgBoxStyle.Critical, "Error Message")
                PictureBoxStatusConnect.Image = My.Resources.Disconnect
            End Try
        ElseIf BtnConnect.Text = "Disconnect" Then
            PictureBoxStatusConnect.Image = My.Resources.Disconnect
            BtnConnect.Text = "Connect"
            LabelConnectionStatus.Text = "Connection Status : Disconnect"
            TimerSerialIn.Stop()
            SerialPort1.Close()
        End If
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs) Handles BtnClear.Click
        LabelID.Text = "ID : ____________"
        LabelName.Text = "Waiting..."
        LabelAddress.Text = "Waiting..."
        LabelCity.Text = "Waiting..."
        LabelCountry.Text = "Waiting..."
        PictureBoxUserImage.Image = Nothing
    End Sub

    Private Sub BtnClear_MouseHover(sender As Object, e As EventArgs) Handles BtnClear.MouseHover
        BtnClear.ForeColor = Color.White
    End Sub

    Private Sub BtnClear_MouseLeave(sender As Object, e As EventArgs) Handles BtnClear.MouseLeave
        BtnClear.ForeColor = Color.FromArgb(6, 71, 165)
    End Sub

    Private Sub BtnClearForm_Click(sender As Object, e As EventArgs) Handles BtnClearForm.Click
        ClearInputUpdateData()
    End Sub

    Private Sub BtnScanID_Click(sender As Object, e As EventArgs) Handles BtnScanID.Click
        If TimerSerialIn.Enabled = True Then
            PanelReadingTagProcess.Visible = True
            GetID = True
            BtnScanID.Enabled = False
        Else
            MsgBox("Failed to open User Data !!!" & vbCr & "Click the Connection menu then click the Connect button.", MsgBoxStyle.Critical, "Error Message")
        End If
    End Sub

    Private Sub BtnClearForm_Leave(sender As Object, e As EventArgs) Handles BtnClearForm.Leave
        BtnClear.ForeColor = Color.FromArgb(6, 71, 165)
    End Sub

    Private Sub BtnClearForm_MouseHover(sender As Object, e As EventArgs) Handles BtnClearForm.MouseHover
        BtnClear.ForeColor = Color.White
    End Sub
    Private Sub BtnScanID_MouseHover(sender As Object, e As EventArgs) Handles BtnClear.MouseHover
        BtnClear.ForeColor = Color.White
    End Sub

    Private Sub BtnScanID_MouseLeave(sender As Object, e As EventArgs) Handles BtnClear.MouseLeave
        BtnClear.ForeColor = Color.FromArgb(6, 71, 165)
    End Sub

    Private Sub PictureBoxImageInput_Click(sender As Object, e As EventArgs) Handles PictureBoxImageInput.Click
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Filter = "JPEG (*.jpeg;*jpg)|*.jpeg;*.jpg"

        If (OpenFileDialog1.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            IMG_FileNameInput = OpenFileDialog1.FileName
            PictureBoxImageInput.ImageLocation = IMG_FileNameInput
        End If
    End Sub

    Private Sub CheckBoxByName_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxByName.CheckedChanged
        If CheckBoxByName.Checked = True Then
            CheckBoxByID.Checked = False
        End If
        If CheckBoxByName.Checked = False Then
            CheckBoxByID.Checked = True
        End If
    End Sub

    Private Sub CheckBoxByID_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxByID.CheckedChanged
        If CheckBoxByID.Checked = True Then
            CheckBoxByName.Checked = False
        End If
        If CheckBoxByID.Checked = False Then
            CheckBoxByName.Checked = True
        End If
    End Sub

    Private Sub TextBoxSearch_TextChanged(sender As Object, e As EventArgs) Handles TextBoxSearch.TextChanged
        If CheckBoxByID.Checked = True Then
            If TextBoxSearch.Text = Nothing Then
                SqlCmdSearchStr = "SELECT Name, ID, Address, City, Country FROM " & Table_Name & " ORDER BY Name"
            Else
                SqlCmdSearchStr = "SELECT Name, ID, Address, City, Country FROM " & Table_Name & " WHERE ID LIKE'" & TextBoxSearch.Text & "%'"
            End If
        End If
        If CheckBoxByName.Checked = True Then
            If TextBoxSearch.Text = Nothing Then
                SqlCmdSearchStr = "SELECT Name, ID, Address, City, Country FROM " & Table_Name & " ORDER BY Name"
            Else
                SqlCmdSearchStr = "SELECT Name, ID, Address, City, Country FROM " & Table_Name & " WHERE Name LIKE'" & TextBoxSearch.Text & "%'"
            End If
        End If

        Try
            Connection.Open()
        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            MySQLDA = New MySqlDataAdapter(SqlCmdSearchstr, Connection)
            DT = New DataTable
            Data = MySQLDA.Fill(DT)
            If Data > 0 Then
                DataGridView1.DataSource = Nothing
                DataGridView1.DataSource = DT
                DataGridView1.DefaultCellStyle.ForeColor = Color.Black
                DataGridView1.ClearSelection()
            Else
                DataGridView1.DataSource = DT
            End If
        Catch ex As Exception
            MsgBox("Failed to search !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
        End Try
        Connection.Close()
    End Sub

    Private Sub DataGridView1_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView1.CellMouseDown
        Try
            If AllCellsSelected(DataGridView1) = False Then
                If e.Button = MouseButtons.Left Then
                    DataGridView1.CurrentCell = DataGridView1(e.ColumnIndex, e.RowIndex)
                    Dim i As Integer
                    With DataGridView1
                        If e.RowIndex >= 0 Then
                            i = .CurrentRow.Index
                            LoadImagesStr = True
                            IDRam = .Rows(i).Cells("ID").Value.ToString
                            ShowData()
                        End If
                    End With
                End If
            End If
        Catch ex As Exception
            Return
        End Try
    End Sub

    Private Function AllCellsSelected(dgv As DataGridView) As Boolean
        AllCellsSelected = (DataGridView1.SelectedCells.Count = (DataGridView1.RowCount * DataGridView1.Columns.GetColumnCount(DataGridViewElementStates.Visible)))
    End Function
    Private Sub TimerTimeDate_Tick(sender As Object, e As EventArgs) Handles TimerTimeDate.Tick
        LabelDateTime.Text = "Time" & DateTime.Now.ToString("HH:mm:ss") & " Date " & DateTime.Now.ToString("dd MMM, yyyy")
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If DataGridView1.RowCount = 0 Then
            MsgBox("Cannot delete, table data is empty", MsgBoxStyle.Critical, "Error Message")
            Return
        End If
        If DataGridView1.SelectedRows.Count = 0 Then
            MsgBox("Cannot delete, select the table data to be deleted", MsgBoxStyle.Critical, "Error Message")
            Return
        End If

        If MsgBox("Delete record?", MsgBoxStyle.Question + MsgBoxStyle.OkCancel, "Confirmation") = MsgBoxResult.Cancel Then Return

        Try
            Connection.Open()
        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            If AllCellsSelected(DataGridView1) = True Then
                MySQLCMD.CommandType = CommandType.Text
                MySQLCMD.CommandText = "DELETE FROM " & Table_Name
                MySQLCMD.Connection = Connection
                MySQLCMD.ExecuteNonQuery()
            End If
            For Each row As DataGridViewRow In DataGridView1.SelectedRows
                If row.Selected = True Then
                    MySQLCMD.CommandType = CommandType.Text
                    MySQLCMD.CommandText = "DELETE FROM " & Table_Name & "WHERE ID='" & row.DataBoundItem(1).ToString & "'"
                    MySQLCMD.Connection = Connection
                    MySQLCMD.ExecuteNonQuery()
                End If
            Next
        Catch ex As Exception
            MsgBox("Failed to delete" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
        End Try
        PictureBoxImagePreview.Image = Nothing
        Connection.Close()
        ShowData()
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs)
        DataGridView1.SelectAll()
    End Sub

    Private Sub ClearToolStripMenuItem_Click(sender As Object, e As EventArgs)
        DataGridView1.ClearSelection()
        PictureBoxImagePreview.Image = Nothing
    End Sub

    Private Sub RefreshToolStripMenuItem1_Click(sender As Object, e As EventArgs)
        ShowData()
    End Sub

    Private Sub TimerSerialIn_Tick(sender As Object, e As EventArgs) Handles TimerSerialIn.Tick
        Try
            StrSerialIn = SerialPort1.ReadExisting
            LabelConnectionStatus.Text = "Connection Status : Connected"
            If StrSerialIn <> "" Then
                If GetID = True Then
                    LabelGetID.Text = StrSerialIn
                    GetID = False
                    If LabelGetID.Text <> "_______" Then
                        PanelReadingTagProcess.Visible = False
                        IDCheck()
                    End If
                End If
                If ViewUserData = True Then
                    ViewData()
                End If
            End If
        Catch ex As Exception
            TimerSerialIn.Stop()
            SerialPort1.Close()
            LabelConnectionStatus.Text = "Connection Status : Disconnected"
            PictureBoxStatusConnect.Image = My.Resources.Disconnect()
            MsgBox("Failed to connect !!!" & vbCr & "Arduino is not detected.", MsgBoxStyle.Critical, "Error Message")
            BtnConnect_Click(sender, e)
            Return
        End Try
        If PictureBoxStatusConnect.Visible = True Then
        ElseIf PictureBoxStatusConnect.Visible = True Then
            PictureBoxStatusConnect.Visible = True
        End If
    End Sub
    Private Sub IDCheck()
        Try
            Connection.Open()
        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            MySQLCMD.CommandType = CommandType.Text
            MySQLCMD.CommandText = "SELECT * FROM " & Table_Name & " WHERE ID LIKE '" & LabelGetID.Text & "'"
            MYSQLDA = New MySqlDataAdapter(MySQLCMD.CommandText, Connection)
            DT = New DataTable
            Data = MySQLDA.Fill(DT)
            If Data > 0 Then
                If MsgBox("ID registred !" & vbCr & "Do you want to edit the data ?", MsgBoxStyle.Question + MsgBoxStyle.OkCancel, "Confirmation") = MsgBoxResult.Cancel Then
                    DT = Nothing
                    Connection.Close()
                    BtnScanID.Enabled = True
                    GetID = False
                    LabelGetID.Text = "_______"
                    Return
                Else
                    Dim ImgArray() As Byte = DT.Rows(0).Item("Images")
                    Dim lmgStr As New System.IO.MemoryStream(ImgArray)
                    PictureBoxImageInput.Image = Image.FromStream(lmgStr)
                    PictureBoxImageInput.SizeMode = PictureBoxSizeMode.Zoom

                    TextBoxName.Text = DT.Rows(0).Item("Name")
                    TextBoxAddress.Text = DT.Rows(0).Item("Address")
                    TextBoxCity.Text = DT.Rows(0).Item("City")
                    TextBoxCountry.Text = DT.Rows(0).Item("Country")
                    StatusInput = "Update"
                End If
            End If
        Catch ex As Exception
            MsgBox("Failed to load Database !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
            Return
        End Try

        DT = Nothing
        Connection.Close()

        BtnScanID.Enabled = True
        GetID = False
    End Sub

    Private Sub ViewData()
        LabelID.Text = "ID : " & StrSerialIn
        If LabelID.Text = "ID : ________" Then
            ViewData()
        Else
            ShowDataUser()
        End If
    End Sub

    Private Sub BtnCloseReadingTag_Click(sender As Object, e As EventArgs) Handles BtnCloseReadingTag.Click
        PanelReadingTagProcess.Visible = False
        BtnScanID.Enabled = True
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        Dim mstream As New System.IO.MemoryStream()
        Dim arrImage() As Byte

        If TextBoxAddress.Text = "" Then
            MessageBox.Show("Address cannot be empty !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If TextBoxCity.Text = "" Then
            MessageBox.Show("Name cannot be empty !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If TextBoxCountry.Text = "" Then
            MessageBox.Show("Country cannot be empty !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If StatusInput = "Save" Then
            If IMG_FileNameInput <> "" Then
                PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)
                arrImage = mstream.GetBuffer()
            Else
                MessageBox.Show("The image has not been selected !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Try
                Connection.Open()
            Catch ex As Exception
                MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try

                Try
                    MySQLCMD = New MySqlCommand
                    With MySQLCMD
                        .CommandText = "INSERT INTO " & Table_Name & "(Name, ID, Address, City, Country, Images) VALUES(@name, @id, @address, @city, @country, @images)"
                        .Connection = Connection
                        .Parameters.AddWithValue("@name", TextBoxName.Text)
                        .Parameters.AddWithValue("@id", LabelGetID.Text)
                        .Parameters.AddWithValue("@address", TextBoxAddress.Text)
                        .Parameters.AddWithValue("@city", TextBoxCity.Text)
                        .Parameters.AddWithValue("@country", TextBoxCountry.Text)
                        .Parameters.AddWithValue("@images", arrImage)
                        .ExecuteNonQuery()
                    End With
                    MsgBox("Data saved successfully", MsgBoxStyle.Information, "Information")
                    IMG_FileNameInput = ""
                    ClearInputUpdateData()
                Catch ex As Exception
                    MsgBox("Data failed to save !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
                    Connection.Close()
                    Return
                End Try
                Connection.Close()
        Else

                If IMG_FileNameInput <> "" Then
                    PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)
                    arrImage = mstream.GetBuffer()

                    Try
                        Connection.Open()
                    Catch ex As Exception
                        MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End Try

                    Try
                        MySQLCMD = New MySqlCommand
                        With MySQLCMD
                            .CommandText = "UPDATE" & Table_Name & " SET Name=@name, ID=@id, Address=@address, City=@city, Country=@country, Images=@images WHERE ID=@id"
                            .Connection = Connection
                            .Parameters.AddWithValue("@name", TextBoxName.Text)
                            .Parameters.AddWithValue("@id", LabelGetID.Text)
                        .Parameters.AddWithValue("@address", TextBoxAddress.Text)
                        .Parameters.AddWithValue("@city", TextBoxCity.Text)
                        .Parameters.AddWithValue("@country", TextBoxCountry.Text)
                        .Parameters.AddWithValue("@images", arrImage)
                            .ExecuteNonQuery()
                        End With
                        MsgBox("Data Updated successfully", MsgBoxStyle.Information, "Information")
                        IMG_FileNameInput = ""
                        BtnSave.Text = "Save"
                        ClearInputUpdateData()
                    Catch ex As Exception
                        MsgBox("Data failed to Update !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
                        Connection.Close()
                        Return
                    End Try
                    Connection.Close()

                Else

                    Try
                        Connection.Open()
                    Catch ex As Exception
                        MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End Try

                    Try
                        MySQLCMD = New MySqlCommand
                        With MySQLCMD
                            .CommandText = "UPDATE" & Table_Name & " SET Name=@name, ID=@id, Address=@address, City=@city, Country=@country, Images=@images WHERE ID=@id"
                            .Connection = Connection
                            .Parameters.AddWithValue("@name", TextBoxName.Text)
                            .Parameters.AddWithValue("@id", LabelGetID.Text)
                            .Parameters.AddWithValue("@address", TextBoxAddress.Text)
                            .Parameters.AddWithValue("@city", TextBoxCity.Text)
                            .Parameters.AddWithValue("@country", TextBoxCountry.Text)
                            .ExecuteNonQuery()
                        End With
                        MsgBox("Data Updated successfully", MsgBoxStyle.Information, "Information")
                        BtnSave.Text = "Save"
                        ClearInputUpdateData()
                    Catch ex As Exception
                        MsgBox("Data failed to Update !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
                        Connection.Close()
                        Return
                    End Try
                    Connection.Close()
                End If
                StatusInput = "Save"
        End If
        PictureBoxImagePreview.Image = Nothing
        ShowData()
    End Sub

    Private Sub BtnConnect_MouseHover(sender As Object, e As EventArgs) Handles BtnScanPort.MouseHover
        BtnScanPort.ForeColor = Color.White

    End Sub

    Private Sub BtnConnect_MouseLeave(sender As Object, e As EventArgs) Handles BtnScanPort.MouseLeave
        BtnScanPort.ForeColor = Color.FromArgb(6, 71, 165)
    End Sub
End Class
