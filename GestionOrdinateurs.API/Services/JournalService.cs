namespace GestionOrdinateurs.API.Services;

public class JournalService
{
    private readonly string _cheminReparations;
    private readonly string _cheminOrdinateurs;
    private readonly object _verrou = new object();

    public JournalService(IWebHostEnvironment environment)
    {
        var dossierReparations = Path.Combine(
            environment.ContentRootPath,
            "Logs",
            "Reparations");

        var dossierOrdinateurs = Path.Combine(
            environment.ContentRootPath,
            "Logs",
            "Ordinateurs"
        );

            Directory.CreateDirectory(dossierReparations);
            Directory.CreateDirectory(dossierOrdinateurs);

            _cheminReparations = Path.Combine(
                dossierReparations,
                "reparation.log");
            _cheminOrdinateurs = Path.Combine(
                dossierOrdinateurs,
                "ordinateurs.log"
            );
    }

    public void EcrireDansFichier(string chemin, string message)
    {
        lock(_verrou)
        {
            using var writer = new StreamWriter(
                chemin,
                append: true);

            writer.WriteLine(
                $"{DateTime.Now:dd/MM/yyyy} - {message}"
            );
        }
    }

    public void EcrireReparation(string message)
    {
        EcrireDansFichier(_cheminReparations, message);
    }

    public void EcrireOrdinateur(string message)
    {
        EcrireDansFichier(_cheminOrdinateurs, message);
    }
}