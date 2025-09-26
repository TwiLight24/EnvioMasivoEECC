Imports System.Data
Imports System.Data.SqlClient

Imports System.Security.Cryptography
Imports System.Text
Module Variables
    Public cn As New SqlConnection
    Public conexRumi As String = "Data Source=192.168.1.2;Initial Catalog=SBO_Rumiimport;User ID=sa;Password=B1Admin"

    Public con As New SqlConnection("Data Source=192.168.1.2;Initial Catalog=SBO_Rumiimport;User ID=sa;Password=B1Admin")

    Public Function damestockpmega1(Parametro As String)
        Dim dt As New DataTable
        Dim da As SqlDataAdapter

        da = New SqlDataAdapter("ListarxParametro2", con)
        da.SelectCommand.CommandType = CommandType.StoredProcedure

        'da.SelectCommand.Parameters.AddWithValue("@Parametro", "CL20604794243")
        da.SelectCommand.Parameters.AddWithValue("@Parametro", Parametro)
        da.Fill(dt)
        Return dt
    End Function

    Public Function DameReporte(ByVal cliente As String) As DataSet
        cn.ConnectionString = conexRumi
        Dim cmd As New SqlCommand
        'Ejecutar el procedure utilizando un DataAdapter
        Dim da As New SqlDataAdapter("BPVS_FIN_REPORTE_CUENTAS_PENDIENTES_CLIENTE_VS3", cn)
        '' Dim da As New SqlDataAdapter("SP_CUENTAS_PENDIENTES_CLIENTE", cn)
        'establecer que el tipo de comando es un Procedimiento Almacenado
        da.SelectCommand.CommandType = CommandType.StoredProcedure

        'definir el parametro @CIUDAD del parametro
        da.SelectCommand.Parameters.AddWithValue("@FECHAINI_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.Parameters.AddWithValue("@FECHAFIN_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.Parameters.AddWithValue("@HASTAFEC_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.Parameters.AddWithValue("@CLIENTE_1", DBNull.Value).Value = cliente
        da.SelectCommand.Parameters.AddWithValue("@ORDEN_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.Parameters.AddWithValue("@Situacion_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.Parameters.AddWithValue("@Banco_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.Parameters.AddWithValue("@Cuenta_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.Parameters.AddWithValue("@MUESTRACHEQUE_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.Parameters.AddWithValue("@TRANSACCION_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.Parameters.AddWithValue("@Tipo_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.Parameters.AddWithValue("@Vendedor_1", DBNull.Value).Value = DBNull.Value
        da.SelectCommand.CommandTimeout = 4000
        'definir un DataTable para almacenar los registros
        Dim ds As New DataSet
        da.Fill(ds)
        Return (ds)
    End Function






End Module