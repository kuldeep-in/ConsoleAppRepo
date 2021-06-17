    #$connection = new-object System.Data.SqlClient.SQLConnection("Data Source=$server;Integrated Security= True;Initial Catalog=$database; MultipleActiveResultSets=True;");
    $connection = new-object System.Data.SqlClient.SQLConnection("Server=tcp:xxx.database.windows.net,1433;Database=xxx;User ID=xxx;Password=xxx; Connection Timeout=5000; MultipleActiveResultSets=True;");
    

    $i = 0;

     while ($i -le 5)
        {
            $a = Get-Date

            write-host -ForegroundColor Green "Loop: $i : $a"

            #$connection.Open();
            #$getspDefQuery = "EXEC [UTIL].[UspOptimizeDatabase]"
            #$spDefcmd = new-object System.Data.SqlClient.SqlCommand($getspDefQuery, $connection);
            #$spDefreader = $spDefcmd.ExecuteReader()

            #$connection.Close(); 
            #$scon = New-Object System.Data.SqlClient.SqlConnection
            #$scon.ConnectionString = $connection
    
            $cmd = New-Object System.Data.SqlClient.SqlCommand
            $cmd.Connection = $connection
            $cmd.CommandText = "EXEC [UTIL].[UspOptimizeDatabase]"
            $cmd.CommandTimeout = 5000
    
            $connection.Open()
            $cmd.ExecuteNonQuery()
            $connection.Close()
            $cmd.Dispose()


            $i++;
            
            Start-Sleep -s 10                
        }

