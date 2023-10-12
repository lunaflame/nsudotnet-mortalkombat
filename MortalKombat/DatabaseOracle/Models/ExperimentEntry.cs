using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Cards;
using Contracts.Interfaces;

namespace Nsu.MortalKombat.DatabaseOracle.Models;

public class ExperimentEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    [Required]
    public Card[] Deck { get; set; } // serialized

    // public IPlayer Player1;
    // public IPlayer Player2;
    
    [Required]
    public int Pick1 { get; set; } = -1;
    [Required]
    public int Pick2 { get; set; } = -1;
    
    [Required]
    public bool AllowFight { get; set; }
}