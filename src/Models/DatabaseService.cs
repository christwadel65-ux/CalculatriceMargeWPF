using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CalculatriceMargeWPF.Models
{
    /// <summary>
    /// Service de gestion de la base de données SQLite pour l'historique
    /// </summary>
    public class DatabaseService : IDisposable
    {
        private readonly string _databasePath;
        private const string DATABASE_FILENAME = "historique.db";
        private bool _disposed = false;

        public DatabaseService(string appDataFolder)
        {
            _databasePath = Path.Combine(appDataFolder, DATABASE_FILENAME);
            InitializeDatabase();
        }

        /// <summary>
        /// Initialise la base de données et crée les tables si nécessaire
        /// </summary>
        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS History (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Titre TEXT NOT NULL,
                    DebourseSec REAL NOT NULL,
                    FraisGeneraux REAL NOT NULL,
                    FraisModeIndex INTEGER NOT NULL,
                    PrixVenteHT REAL NOT NULL,
                    RemisePourcentage REAL DEFAULT 0,
                    TVA REAL NOT NULL,
                    PrixRevient REAL NOT NULL,
                    MargeBrute REAL NOT NULL,
                    MargeBrutePourcentage REAL NOT NULL,
                    MargeNette REAL NOT NULL,
                    MargeNettePourcentage REAL NOT NULL,
                    PrixVenteTTC REAL NOT NULL,
                    DateCalcul TEXT NOT NULL
                );

                CREATE INDEX IF NOT EXISTS idx_date ON History(DateCalcul);
                CREATE INDEX IF NOT EXISTS idx_titre ON History(Titre);
            ";
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Ajoute une entrée dans l'historique
        /// </summary>
        public long AddEntry(HistoryEntry entry)
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO History (Titre, DebourseSec, FraisGeneraux, FraisModeIndex, 
                                    PrixVenteHT, RemisePourcentage, TVA, PrixRevient, MargeBrute, 
                                    MargeBrutePourcentage, MargeNette, MargeNettePourcentage,
                                    PrixVenteTTC, DateCalcul)
                VALUES ($titre, $debourseSec, $fraisGeneraux, $fraisModeIndex,
                        $prixVenteHT, $remisePourcentage, $tva, $prixRevient, $margeBrute,
                        $margeBrutePourcentage, $margeNette, $margeNettePourcentage,
                        $prixVenteTTC, $dateCalcul)
            ";

            command.Parameters.AddWithValue("$titre", entry.Titre);
            command.Parameters.AddWithValue("$debourseSec", entry.DebourseSec);
            command.Parameters.AddWithValue("$fraisGeneraux", entry.FraisGeneraux);
            command.Parameters.AddWithValue("$fraisModeIndex", entry.FraisModeIndex);
            command.Parameters.AddWithValue("$prixVenteHT", entry.PrixVenteHT);
            command.Parameters.AddWithValue("$remisePourcentage", entry.RemisePourcentage);
            command.Parameters.AddWithValue("$tva", entry.TVA);
            command.Parameters.AddWithValue("$prixRevient", entry.PrixRevient);
            command.Parameters.AddWithValue("$margeBrute", entry.MargeBrute);
            command.Parameters.AddWithValue("$margeBrutePourcentage", entry.MargeBrutePourcentage);
            command.Parameters.AddWithValue("$margeNette", entry.MargeNette);
            command.Parameters.AddWithValue("$margeNettePourcentage", entry.MargeNettePourcentage);
            command.Parameters.AddWithValue("$prixVenteTTC", entry.PrixVenteTTC);
            command.Parameters.AddWithValue("$dateCalcul", entry.DateCalcul.ToString("yyyy-MM-dd HH:mm:ss"));

            command.ExecuteNonQuery();

            command.CommandText = "SELECT last_insert_rowid()";
            return (long)command.ExecuteScalar();
        }

        /// <summary>
        /// Récupère toutes les entrées d'historique, triées par date (plus récentes en premier)
        /// </summary>
        public List<HistoryEntry> GetAllEntries()
        {
            var entries = new List<HistoryEntry>();

            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Titre, DebourseSec, FraisGeneraux, FraisModeIndex,
                       PrixVenteHT, RemisePourcentage, TVA, PrixRevient, MargeBrute, MargeBrutePourcentage,
                       MargeNette, MargeNettePourcentage, PrixVenteTTC, DateCalcul
                FROM History
                ORDER BY DateCalcul DESC
            ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                entries.Add(new HistoryEntry
                {
                    Id = reader.GetInt32(0),
                    Titre = reader.GetString(1),
                    DebourseSec = reader.GetDouble(2),
                    FraisGeneraux = reader.GetDouble(3),
                    FraisModeIndex = reader.GetInt32(4),
                    PrixVenteHT = reader.GetDouble(5),
                    RemisePourcentage = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                    TVA = reader.GetDouble(7),
                    PrixRevient = reader.GetDouble(8),
                    MargeBrute = reader.GetDouble(9),
                    MargeBrutePourcentage = reader.GetDouble(10),
                    MargeNette = reader.GetDouble(11),
                    MargeNettePourcentage = reader.GetDouble(12),
                    PrixVenteTTC = reader.GetDouble(13),
                    DateCalcul = DateTime.Parse(reader.GetString(14))
                });
            }

            return entries;
        }

        /// <summary>
        /// Supprime une entrée par son ID
        /// </summary>
        public bool DeleteEntry(int id)
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM History WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);

            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Supprime toutes les entrées
        /// </summary>
        public void ClearHistory()
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM History";
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Récupère une entrée par son ID
        /// </summary>
        public HistoryEntry GetEntryById(int id)
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Titre, DebourseSec, FraisGeneraux, FraisModeIndex,
                       PrixVenteHT, RemisePourcentage, TVA, PrixRevient, MargeBrute, MargeBrutePourcentage,
                       MargeNette, MargeNettePourcentage, PrixVenteTTC, DateCalcul
                FROM History
                WHERE Id = $id
            ";
            command.Parameters.AddWithValue("$id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new HistoryEntry
                {
                    Id = reader.GetInt32(0),
                    Titre = reader.GetString(1),
                    DebourseSec = reader.GetDouble(2),
                    FraisGeneraux = reader.GetDouble(3),
                    FraisModeIndex = reader.GetInt32(4),
                    PrixVenteHT = reader.GetDouble(5),
                    RemisePourcentage = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                    TVA = reader.GetDouble(7),
                    PrixRevient = reader.GetDouble(8),
                    MargeBrute = reader.GetDouble(9),
                    MargeBrutePourcentage = reader.GetDouble(10),
                    MargeNette = reader.GetDouble(11),
                    MargeNettePourcentage = reader.GetDouble(12),
                    PrixVenteTTC = reader.GetDouble(13),
                    DateCalcul = DateTime.Parse(reader.GetString(14))
                };
            }

            return null;
        }

        /// <summary>
        /// Recherche des entrées par titre (recherche partielle)
        /// </summary>
        public List<HistoryEntry> SearchByTitle(string searchTerm)
        {
            var entries = new List<HistoryEntry>();

            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Titre, DebourseSec, FraisGeneraux, FraisModeIndex,
                       PrixVenteHT, RemisePourcentage, TVA, PrixRevient, MargeBrute, MargeBrutePourcentage,
                       MargeNette, MargeNettePourcentage, PrixVenteTTC, DateCalcul
                FROM History
                WHERE Titre LIKE $searchTerm
                ORDER BY DateCalcul DESC
            ";
            command.Parameters.AddWithValue("$searchTerm", $"%{searchTerm}%");

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                entries.Add(new HistoryEntry
                {
                    Id = reader.GetInt32(0),
                    Titre = reader.GetString(1),
                    DebourseSec = reader.GetDouble(2),
                    FraisGeneraux = reader.GetDouble(3),
                    FraisModeIndex = reader.GetInt32(4),
                    PrixVenteHT = reader.GetDouble(5),
                    RemisePourcentage = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                    TVA = reader.GetDouble(7),
                    PrixRevient = reader.GetDouble(8),
                    MargeBrute = reader.GetDouble(9),
                    MargeBrutePourcentage = reader.GetDouble(10),
                    MargeNette = reader.GetDouble(11),
                    MargeNettePourcentage = reader.GetDouble(12),
                    PrixVenteTTC = reader.GetDouble(13),
                    DateCalcul = DateTime.Parse(reader.GetString(14))
                });
            }

            return entries;
        }

        /// <summary>
        /// Récupère une entrée par son titre exact
        /// </summary>
        public HistoryEntry GetEntryByTitle(string titre)
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Titre, DebourseSec, FraisGeneraux, FraisModeIndex,
                       PrixVenteHT, RemisePourcentage, TVA, PrixRevient, MargeBrute, MargeBrutePourcentage,
                       MargeNette, MargeNettePourcentage, PrixVenteTTC, DateCalcul
                FROM History
                WHERE Titre = $titre
                LIMIT 1
            ";
            command.Parameters.AddWithValue("$titre", titre);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new HistoryEntry
                {
                    Id = reader.GetInt32(0),
                    Titre = reader.GetString(1),
                    DebourseSec = reader.GetDouble(2),
                    FraisGeneraux = reader.GetDouble(3),
                    FraisModeIndex = reader.GetInt32(4),
                    PrixVenteHT = reader.GetDouble(5),
                    RemisePourcentage = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                    TVA = reader.GetDouble(7),
                    PrixRevient = reader.GetDouble(8),
                    MargeBrute = reader.GetDouble(9),
                    MargeBrutePourcentage = reader.GetDouble(10),
                    MargeNette = reader.GetDouble(11),
                    MargeNettePourcentage = reader.GetDouble(12),
                    PrixVenteTTC = reader.GetDouble(13),
                    DateCalcul = DateTime.Parse(reader.GetString(14))
                };
            }

            return null;
        }

        /// <summary>
        /// Met à jour une entrée existante
        /// </summary>
        public bool UpdateEntry(HistoryEntry entry)
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE History SET
                    Titre = $titre,
                    DebourseSec = $debourseSec,
                    FraisGeneraux = $fraisGeneraux,
                    FraisModeIndex = $fraisModeIndex,
                    PrixVenteHT = $prixVenteHT,
                    RemisePourcentage = $remisePourcentage,
                    TVA = $tva,
                    PrixRevient = $prixRevient,
                    MargeBrute = $margeBrute,
                    MargeBrutePourcentage = $margeBrutePourcentage,
                    MargeNette = $margeNette,
                    MargeNettePourcentage = $margeNettePourcentage,
                    PrixVenteTTC = $prixVenteTTC,
                    DateCalcul = $dateCalcul
                WHERE Id = $id
            ";

            command.Parameters.AddWithValue("$id", entry.Id);
            command.Parameters.AddWithValue("$titre", entry.Titre);
            command.Parameters.AddWithValue("$debourseSec", entry.DebourseSec);
            command.Parameters.AddWithValue("$fraisGeneraux", entry.FraisGeneraux);
            command.Parameters.AddWithValue("$fraisModeIndex", entry.FraisModeIndex);
            command.Parameters.AddWithValue("$prixVenteHT", entry.PrixVenteHT);
            command.Parameters.AddWithValue("$remisePourcentage", entry.RemisePourcentage);
            command.Parameters.AddWithValue("$tva", entry.TVA);
            command.Parameters.AddWithValue("$prixRevient", entry.PrixRevient);
            command.Parameters.AddWithValue("$margeBrute", entry.MargeBrute);
            command.Parameters.AddWithValue("$margeBrutePourcentage", entry.MargeBrutePourcentage);
            command.Parameters.AddWithValue("$margeNette", entry.MargeNette);
            command.Parameters.AddWithValue("$margeNettePourcentage", entry.MargeNettePourcentage);
            command.Parameters.AddWithValue("$prixVenteTTC", entry.PrixVenteTTC);
            command.Parameters.AddWithValue("$dateCalcul", entry.DateCalcul.ToString("yyyy-MM-dd HH:mm:ss"));

            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Récupère les statistiques de l'historique
        /// </summary>
        public (int Count, double TotalMargeBrute, double TotalMargeNette, double AvgMargeBrutePct, double AvgMargeNettePct) GetStatistics()
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*), 
                       SUM(MargeBrute), 
                       SUM(MargeNette),
                       AVG(MargeBrutePourcentage),
                       AVG(MargeNettePourcentage)
                FROM History
            ";

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (
                    reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    reader.IsDBNull(1) ? 0 : reader.GetDouble(1),
                    reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                    reader.IsDBNull(3) ? 0 : reader.GetDouble(3),
                    reader.IsDBNull(4) ? 0 : reader.GetDouble(4)
                );
            }

            return (0, 0, 0, 0, 0);
        }

        // ============================
        // MÉTHODES ASYNCHRONES
        // ============================

        /// <summary>
        /// Récupère toutes les entrées de manière asynchrone
        /// </summary>
        public async Task<List<HistoryEntry>> GetAllEntriesAsync()
        {
            return await Task.Run(() => GetAllEntries());
        }

        /// <summary>
        /// Ajoute une entrée de manière asynchrone
        /// </summary>
        public async Task<long> AddEntryAsync(HistoryEntry entry)
        {
            return await Task.Run(() => AddEntry(entry));
        }

        /// <summary>
        /// Supprime une entrée de manière asynchrone
        /// </summary>
        public async Task<bool> DeleteEntryAsync(int id)
        {
            return await Task.Run(() => DeleteEntry(id));
        }

        /// <summary>
        /// Récupère les statistiques de manière asynchrone
        /// </summary>
        public async Task<(int Count, double TotalMargeBrute, double TotalMargeNette, double AvgMargeBrutePct, double AvgMargeNettePct)> GetStatisticsAsync()
        {
            return await Task.Run(() => GetStatistics());
        }

        /// <summary>
        /// Récupère une entrée par titre de manière asynchrone
        /// </summary>
        public async Task<HistoryEntry> GetEntryByTitleAsync(string titre)
        {
            return await Task.Run(() => GetEntryByTitle(titre));
        }

        /// <summary>
        /// Met à jour une entrée de manière asynchrone
        /// </summary>
        public async Task<bool> UpdateEntryAsync(HistoryEntry entry)
        {
            return await Task.Run(() => UpdateEntry(entry));
        }

        /// <summary>
        /// Libère les ressources utilisées par le service
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Fermer toutes les connexions SQLite en attente
                    SqliteConnection.ClearAllPools();
                }
                _disposed = true;
            }
        }

        ~DatabaseService()
        {
            Dispose(false);
        }
    }
}
