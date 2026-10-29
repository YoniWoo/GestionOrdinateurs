using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestionOrdinateurs.Shared
{
    [Table("Ordinateur")]
    public class Ordinateur
    {
        public int Id { get; init; }

        public string Nom { get; private set; } = "";

        public EtatOrdinateur Etat { get; private set; } = EtatOrdinateur.EnService;

        public int Stock { get; private set; } = 0;

        public int Prix { get; private set; } = 0;

        public bool Vendu { get; private set; } = false;

        public Caracteristiques Caracteristiques { get; set; } 
            = new Caracteristiques(0, TypeOrdinateur.Fixe, 0, "", "");

        [JsonIgnore]
        public List<Reparation> Reparations { get; set; } = new();

        public void ChangerNom(string nouveauNom)
        {
            if (nouveauNom is null || nouveauNom.Trim() == "")
            {
                throw new ArgumentException(
                    "Le nom ne peut pas être vide.", nameof(nouveauNom));
            }
            Nom = nouveauNom.Trim();
        }

        public void ChangerMarque(string nouvelleMarque)
        {
            if (nouvelleMarque is null || nouvelleMarque.Trim() == "")
            {
                throw new ArgumentException(
                    "La marque ne peut pas être vide.", nameof(nouvelleMarque));
            }
            Caracteristiques.Marque = nouvelleMarque.Trim();
        }

        public void ChangerRAM(int nouvelleQuantiteGo)
        {
            if (nouvelleQuantiteGo <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(nouvelleQuantiteGo),
                        "La quantité de RAM doit être supérieure à zéro.");
                }
            Caracteristiques.RamGo = nouvelleQuantiteGo;
        }
        
        public void ChangerProcesseur(string nouveauProcesseur)
        {
            if (nouveauProcesseur == null || nouveauProcesseur.Trim() == "")
            {
                throw new ArgumentException(
                    "Le processeur ne peut pas être vide.", nameof(nouveauProcesseur));
            }
            Caracteristiques.Processeur = nouveauProcesseur.Trim();
        }

        public void ChangerEtat(EtatOrdinateur nouvelEtat)
        {
            if (!Enum.IsDefined(typeof(EtatOrdinateur), nouvelEtat))
            {
                throw new ArgumentException(
                    "L'état spécifié n'est pas valide.", nameof(nouvelEtat));
            }
            Etat = nouvelEtat;
        }
        public void ChangerStock(int nouvelleQuantite)
        {
            if (nouvelleQuantite < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(nouvelleQuantite),
                    "La quantité en stock ne peut pas être négative.");
            }
            Stock = nouvelleQuantite;
        }

        public void ChangerPrix(int nouveauPrix)
        {
            if (nouveauPrix < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(nouveauPrix),
                    "Le prix ne peut pas être négatif.");
            }
            Prix = nouveauPrix;
        }

        public void ChangerTypeOrdinateur(TypeOrdinateur nouveauType)
        {
            if (!Enum.IsDefined(typeof(TypeOrdinateur), nouveauType))
            {
                throw new ArgumentException(
                    "Le type d'ordinateur spécifié n'est pas valide.", nameof(nouveauType));
            }
            Caracteristiques.TypeOrdi = nouveauType;
        }
        public void ChangerStockage(int nouvelleCapaciteGo)
        {
            if (nouvelleCapaciteGo <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(nouvelleCapaciteGo),
                    "La capacité de stockage doit être supérieure à zéro.");
            }
            Caracteristiques.StockageGo = nouvelleCapaciteGo;
        }
    }

}