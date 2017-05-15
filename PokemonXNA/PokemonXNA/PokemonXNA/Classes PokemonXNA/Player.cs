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

    public class Player : Trainer
    {
        public const int DISTANCE_MOD�LE_CAM�RA = 1;
        const float vitesseD�placement = 1f;
        const float HAUTEUR_CAM�RA = 2f;
        const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degr� � la fois
        const float DELTA_LACET = MathHelper.Pi / 180; // 1 degr� � la fois
        const float D�PLACEMEMENT_MOD�LE = /*0.05f*/1f;
        public float Hauteur { get; private set; }
        TerrainAvecBase Terrain { get; set; }
        const float VitesseRotation = 5f;
        public Vector2 Souris { get; private set; }

        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        float Rayon { get; set; }
        public Vector3 UpPositionTrainer { get; set; }
        float positionMilieuHauteurterrain;

        protected InputManager GestionInput { get; private set; }
        bool inventaireOuvert;

        BasicEffect EffetDeBase { get; set; }
        Vector3 Direction { get; set; }
        Vector3 OrientationVertical { get; } = Vector3.Up;
        Vector3 Lat�ral { get; set; }
        float AngleDirection { get; set; }
        public Vector3 DirectionCam�ra { get; private set; }

        public Player(Game jeu, string nomMod�le, float �chelle, Vector3 rotation, Vector3 position, float intervallleMAJ, float rayon)
            : base(jeu, "PLAYER", nomMod�le, �chelle, rotation, position)
        {
            IntervalleMAJ = intervallleMAJ;
            Rayon = rayon;

            Sph�reDeCollision = new BoundingSphere(position, Rayon);
            Hauteur = 2000 * Rayon * �chelle;
        }

        public override void Initialize()
        {
            inventaireOuvert = false;
            UpPositionTrainer = Vector3.Up + Position;
            Temps�coul�DepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
            positionMilieuHauteurterrain = Terrain.HAUTEUR_MAXIMALE / 2;
        }

        public override void Update(GameTime gameTime)
        {
            InventairePoks();

            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                EffectuerMise�Jour();
                Temps�coul�DepuisMAJ = 0;
            }
            Souris = new Vector2(GestionInput.GetPositionSouris().X, GestionInput.GetPositionSouris().Y);
            Sph�reDeCollision = new BoundingSphere(Position, Sph�reDeCollision.Radius);
        }

        protected override void EffectuerMise�Jour()
        {
            if (!(Combat.EnCombat || AfficheurTexte.MessageEnCours))
            {
                Bouger();
                TournerTrainer();
            }
            Cam�raJeu.Position = new Vector3(Position.X + 3, Position.Y + HAUTEUR_CAM�RA, Position.Z - 3);
            CalculerMonde();

        }

        private void InventairePoks()
        {
            if ((GestionInput.EstNouvelleTouche(Keys.P) || GestionInput.EstNouveauY_inventaire())&& !inventaireOuvert && !Combat.EnCombat)
            {
                foreach (TexteFixe t in Game.Components.Where(t => t is TexteFixe))
                {
                    t.�D�truire = true;
                }
                string InventaireParLigne = null;
                for (int i = 0; i < GetNbPokemon; i++)
                {
                    InventaireParLigne = GetPokemon(i).Nom + " Level : " + GetPokemon(i).Level + " Type : " + GetPokemon(i).Type1;  
                    if(GetPokemon(i).Type2 != "Null")
                    {
                        InventaireParLigne += "/" + GetPokemon(i).Type2;
                        //InventaireParLigne += " Type2 : " + GetPokemon(i).Type2;
                    }
                    InventaireParLigne += " HP : " + GetPokemon(i).HP + "/" + GetPokemon(i).MaxHp +" Exp : " +GetPokemon(i).Exp + "/" + Math.Round(GetPokemon(i).CalculerExpTotal(GetPokemon(i).Level + 1), 0);

                    Game.Components.Add(new TexteFixe(Game, new Vector2(1, 1 + i * 16), InventaireParLigne));
                }
            inventaireOuvert = !inventaireOuvert;
            }
            else if  ((GestionInput.EstNouvelleTouche(Keys.P) || GestionInput.EstNouveauY_inventaire()) && inventaireOuvert)
            {
                foreach (TexteFixe t in Game.Components.Where(t => t is TexteFixe))
                {
                    t.�D�truire = true;
                }
                inventaireOuvert = !inventaireOuvert;
            }
        }

        private void TournerTrainer()
        {
            if(GamePad.GetState(PlayerIndex.One).IsConnected && GestionInput.EstPench�())
            {
                float valYaw = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X;
                //float valPitch = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y;
                Modificationangle(valYaw);
            }

            //d�placement horizontale Angle # pas de limite
            if (GestionInput.GetPositionSouris().X != Souris.X)
            {            
                float valYaw = GestionInput.GetPositionSouris().X > Souris.X ? 1 : -1;
                //int valPitch = GestionInput.GetPositionSouris().Y > Souris.Y ? 1 : -1;
                Modificationangle(valYaw);
            }
        }
        void Modificationangle(float valeurDetour) 
        {
                Vector3 DirectionJoueur = Monde.Forward - Monde.Backward;
                float valRotationAjouter = DELTA_LACET * valeurDetour * VitesseRotation;
                ((Cam�raJeu) as Cam�raSubjective).Direction = Vector3.Normalize(Vector3.Transform(((Cam�raJeu) as Cam�raSubjective).Direction, Matrix.CreateFromAxisAngle(((Cam�raJeu) as Cam�raSubjective).OrientationVerticale, valRotationAjouter)));
                DirectionCam�ra = ((Cam�raJeu) as Cam�raSubjective).Direction; // Pour qu'on puisse avoir acc�s � la direction de la cam�ra dans la classe pok�ball 
                DirectionCam�ra = Vector3.Normalize(DirectionCam�ra);
                Rotation = new Vector3(0, Rotation.Y + valRotationAjouter, 0);
        }
        protected void BougerCam�ra(float d�placementHorizontal, float d�placementProfondeur)
        {
            ((Cam�raJeu) as Cam�raSubjective).G�rerD�placement(d�placementProfondeur, d�placementHorizontal);
            Cam�raJeu.Cr�erPointDeVue(Cam�raJeu.Position, new Vector3(Position.X, Position.Y + 2f, Position.Z), Cam�raJeu.OrientationVerticale);
        }
        protected void Bouger()
        {
            if (GestionInput.EstClavierActiv�)
            {
                float d�placementHorizontal = (G�rerTouche(Keys.D) - G�rerTouche(Keys.A)) * vitesseD�placement;
                float d�placementProfondeur = (G�rerTouche(Keys.W) - G�rerTouche(Keys.S)) * vitesseD�placement;
                if (d�placementHorizontal != 0 || d�placementProfondeur != 0)
                {
                    BougerTrainer(d�placementHorizontal, d�placementProfondeur);
                    BougerCam�ra(d�placementHorizontal, d�placementProfondeur);
                }
            }
            if(GamePad.GetState(PlayerIndex.One).IsConnected) 
            {
                float d�placementHorizontal = (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X) * vitesseD�placement;
                float d�placementProfondeur = (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y) * vitesseD�placement;
                if (d�placementHorizontal != 0 || d�placementProfondeur != 0)
                {
                    //GamePad.SetVibration(PlayerIndex.One, 1, 1); omg cest intense!
                    BougerTrainer(d�placementHorizontal, d�placementProfondeur);
                    BougerCam�ra(d�placementHorizontal, d�placementProfondeur);
                }
            }
        }
        protected void BougerTrainer(float d�placementHorizontal, float d�placementProfondeur)
        {
            Direction = ((Cam�raJeu) as Cam�raSubjective).Direction;
            Lat�ral = Vector3.Cross(Direction, OrientationVertical);

            Position += Direction * d�placementProfondeur;
            Position += Lat�ral * d�placementHorizontal;
            Limites();

            Vector2 vecteurPosition = new Vector2(Position.X + Terrain.NbColonnes / 2, Position.Z + Terrain.NbRang�es / 2);
            float posY = (Terrain.GetPointSpatial((int)Math.Round(vecteurPosition.X, 0), Terrain.NbRang�es - (int)Math.Round(vecteurPosition.Y, 0)) + Vector3.Zero).Y;
            Position = new Vector3(Position.X, posY, Position.Z);

            Sph�reDeCollision = new BoundingSphere(Position, Sph�reDeCollision.Radius);

        }
        private void Limites()
        {
            Position = new Vector3(MathHelper.Max(MathHelper.Min(Position.X, Terrain.NbColonnes / 2), -Terrain.NbColonnes / 2), Position.Y,
             MathHelper.Max(MathHelper.Min(Position.Z, Terrain.NbRang�es / 2), -Terrain.NbRang�es / 2));
        }

        float G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? 1 : 0;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}