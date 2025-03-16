using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;  // Add on
using Mono.Data.Sqlite;  // Add on

public class SQLiteAdapter : MonoBehaviour
{
    public string DBFileName = "player_wallet.db";  // ชื่อไฟล์ฐานข้อมูล
    public string DBFolder = "Plugins";  // โฟลเดอร์ที่เก็บฐานข้อมูล

    private IDbConnection dbcon;
    private IDbCommand dbcommd;

    // ฟังก์ชันเชื่อมต่อฐานข้อมูล
    public void ConnectDatabase()
    {
        string connectionString = "URI=file:" + Application.dataPath + "/" + this.DBFolder + "/" + this.DBFileName;

        this.dbcon = new SqliteConnection(connectionString);
        this.dbcon.Open();
        this.dbcommd = this.dbcon.CreateCommand();
        Debug.Log("Database connected successfully.");
    }

    // ฟังก์ชันปิดการเชื่อมต่อฐานข้อมูล
    public void DisconnectDatabase()
    {
        if (this.dbcon != null && this.dbcon.State == ConnectionState.Open)
        {
            this.dbcommd.Dispose();  // ปิดคำสั่งที่ใช้งาน
            this.dbcon.Close();      // ปิดการเชื่อมต่อ
            this.dbcon.Dispose();    // กำจัดอ็อบเจ็กต์
            Debug.Log("Database disconnected successfully.");
        }
        else
        {
            Debug.LogWarning("Database was not open or already disconnected.");
        }
    }

    // ฟังก์ชันอัพเดตเหรียญในฐานข้อมูล
    public void UpdateCoinsInDatabase(int coins)
    {
        this.dbcommd.CommandText = "UPDATE player_wallet SET balance = " + coins + " WHERE id = 1";

        try
        {
            this.dbcommd.ExecuteNonQuery();  // รันคำสั่ง SQL
            Debug.Log("Coins updated in database: " + coins);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to update coins in database: " + ex.Message);
        }
    }

    // ฟังก์ชันดึงข้อมูลเหรียญจากฐานข้อมูล
    public int GetCoinsFromDatabase()
    {
        int coins = 0;
        try
        {
            // รันคำสั่ง SQL เพื่อดึงค่า balance จากฐานข้อมูล
            this.dbcommd.CommandText = "SELECT balance FROM player_wallet WHERE id = 1";
            IDataReader reader = this.dbcommd.ExecuteReader();

            if (reader.Read()) // อ่านข้อมูลจากฐานข้อมูล
            {
                coins = reader.GetInt32(0);  // รับค่า balance จากคอลัมน์แรก
            }
            reader.Close();
            Debug.Log("Coins retrieved from database: " + coins);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to retrieve coins from database: " + ex.Message);
        }
        return coins;
    }
}
