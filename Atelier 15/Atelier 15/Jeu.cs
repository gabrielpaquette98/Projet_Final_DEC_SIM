using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA
{
        enum États { JEU3D, PAGE_TITRE, COMBAT, MAISON, GYM, FIN }
    public class Jeu : Microsoft.Xna.Framework.GameComponent
    {
        États ÉtatJeu { get; set; }
        public Jeu(Game game)
            : base(game)
        {

        }
        public override void Initialize()
        {
            ÉtatJeu = Game.Services.GetService(typeof(États)) as États;
            base.Initialize();
            ÉtatJeu = États.PAGE_TITRE;
        }
        public override void Update(GameTime gameTime)
        {
            GérerClavier();
            GérerTransition();
            GérerÉtat(); 
            base.Update(gameTime);
        }
        private void GérerTransition()
        {
            switch ()
            {
                case États.PAGE_TITRE:
                    GérerTransitionPageTitre();
                    break;
                case États.JEU3D:
                    GérerTransitionJEU3D();
                    break;
                case États.COMBAT:
                    GérerTransitionCombat();
                    break;
                case États.MAISON:
                    GérerTransitionMaison();
                    break;
                case États.GYM:
                    GérerTransitionGym();
                    break;
                case États.FIN:
                    GérerTransitionFin();
                    break;
                default:
                    break;
            }
        }
        private void GérerÉtat()
        {
            switch (ÉtatJeu)
            {
                case États.PAGE_TITRE:
                    PageTitre();
                    break;
                case États.JEU3D:
                    GérerCollision();
                    GérerCombat();
                    GérerComputer();
                    break;
                case États.COMBAT:
                    Combat();
                    break;
                case États.MAISON:
                    GérerCollision();
                    GérerVitesseDéplacement();
                    GérerComputer();
                    break;
                case États.GYM:
                    GérerVitesseDéplacement();
                    GérerComputer();
                    GérerCombat();
                    break;
                default: //États.FIN:
                    Fin();
                    SauvegardeAuto();
                    break;
            }
        }
    }
}
