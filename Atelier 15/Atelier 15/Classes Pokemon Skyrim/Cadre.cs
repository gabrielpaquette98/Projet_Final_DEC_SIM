﻿//Gabriel Paquette, Mars 2017
//Modifications 2 Avril 2017: finalisation, changement de la manière de faire les tableaux
//              5 Avril 2017: Simplification de CréerTableauTextures (les arrêtes se font en même temps)
//État: Fonctionnel, non nettoyé, il faudrait changer la manière de faire le cadre car avoir autant de boucles "for", c'est un peu laid
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace AtelierXNA
{
    public class Cadre : Microsoft.Xna.Framework.DrawableGameComponent, IDestructible
    {
        public const int LARGEUR_BOX_STANDARD = 32;
        public const int HAUTEUR_BOX_STANDARD = 6;
        public const int TAILLE_TILE = 16;
        protected bool Clignotant { get; set; }
        string NomImage { get; set; }
        protected Vector2 Position { get; set; }
        protected Texture2D TextureCadre { get; private set; }
        protected SpriteBatch GestionSprites { get; set; }

        protected List<Rectangle> RectangleSourcesCadre { get; set; }

        protected Rectangle[,] PtsTile { get; private set; }
        protected Vector2[,] Positions { get; private set; }

        protected int NBLignes { get; set; }
        protected int NBColonnes { get; set; }

        public bool ÀDétruire { get; set; }

        
        public Cadre(Game jeu, Vector2 position, int largeur, int hauteur)
           : base(jeu)
        {
            NomImage = "Cadre48x64";//Parce que tout le jeu utilise le même cadre, je ne le demande pas à l'instanciation
            Position = position;
            NBLignes = hauteur;
            NBColonnes = largeur;
        }
        public override void Initialize()
        {
            PtsTile = new Rectangle[NBColonnes, NBLignes];
            Positions = new Vector2[NBColonnes, NBLignes];
            RectangleSourcesCadre = new List<Rectangle>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            TextureCadre = Game.Content.Load<Texture2D>("Textures/" + NomImage);
            CréerListeTiles();
            CréerTableauPositions();
            CréerTableauTextures();
        }


        private void CréerListeTiles()
        {
            //Faudrait faire une boucle selon le nb de tiles de la texture(important, le code live est laid pis c'est pas toff)
            RectangleSourcesCadre.Add(new Rectangle(0, 0, TAILLE_TILE, TAILLE_TILE)); // A = 0; i = 0; j = 0
            RectangleSourcesCadre.Add(new Rectangle(TAILLE_TILE, 0, TAILLE_TILE, TAILLE_TILE)); // A² = 1; i = 1; j = 0
            RectangleSourcesCadre.Add(new Rectangle(TAILLE_TILE * 2, 0, TAILLE_TILE, TAILLE_TILE)); // A³ = 2

            RectangleSourcesCadre.Add(new Rectangle(0, TAILLE_TILE, TAILLE_TILE, TAILLE_TILE)); //Ligne B = 3
            RectangleSourcesCadre.Add(new Rectangle(TAILLE_TILE, TAILLE_TILE, TAILLE_TILE, TAILLE_TILE)); // B² = 4 BLANC
            RectangleSourcesCadre.Add(new Rectangle(TAILLE_TILE * 2, TAILLE_TILE, TAILLE_TILE, TAILLE_TILE)); // B³ = 5

            RectangleSourcesCadre.Add(new Rectangle(0, TAILLE_TILE * 2, TAILLE_TILE, TAILLE_TILE)); //Ligne C = 6
            RectangleSourcesCadre.Add(new Rectangle(TAILLE_TILE, TAILLE_TILE * 2, TAILLE_TILE, TAILLE_TILE)); // C² = 7
            RectangleSourcesCadre.Add(new Rectangle(TAILLE_TILE * 2, TAILLE_TILE * 2, TAILLE_TILE, TAILLE_TILE)); // C³ = 8
                                                                                                                 //On a une liste des tiles qu'on va utiliser pour former un affichage
            RectangleSourcesCadre.Add(new Rectangle(0, TAILLE_TILE * 3, TAILLE_TILE, TAILLE_TILE)); //Ligne D = 9
            RectangleSourcesCadre.Add(new Rectangle(TAILLE_TILE, TAILLE_TILE * 3, TAILLE_TILE, TAILLE_TILE));
        }

        private void CréerTableauPositions()
        {
            for (int j = 0; j < NBLignes; j++)
            {
                for (int i = 0; i < NBColonnes; i++)
                {
                    Positions[i, j] = new Vector2(Position.X + (TAILLE_TILE) * i, Position.Y + (TAILLE_TILE) * j);
                }
            }
        }


        private void CréerTableauTextures()
        {

            for (int j = 1; j < NBLignes - 1; j++)//intérieur blanc
            {
                for (int i = 1; i < NBColonnes - 1; i++)
                {
                    PtsTile[i, j] = RectangleSourcesCadre[4];
                }
            }
            //Si on change l'ordre des tiles, on pourrait trouver un moyen de simplifier plus le code? (parce qu'au moins les numéros de tile sont arbitraires)
            PtsTile[0, 0] = RectangleSourcesCadre[0];//Coins
            PtsTile[NBColonnes - 1, 0] = RectangleSourcesCadre[2];
            PtsTile[0, NBLignes - 1] = RectangleSourcesCadre[6];
            PtsTile[NBColonnes - 1, NBLignes - 1] = RectangleSourcesCadre[8];

            //Arrêtes
            for (int i = 1; i < NBColonnes - 1; i++) //Lignes de cadre horizontal (haut/bas)
            {
                PtsTile[i, 0] = RectangleSourcesCadre[1];
                PtsTile[i, NBLignes - 1] = RectangleSourcesCadre[7];
            }            

            for (int j = 1; j < NBLignes - 1; j++) //Lignes de cadre verticales (gauche/droite)
            {
                PtsTile[0, j] = RectangleSourcesCadre[3];
                PtsTile[NBColonnes - 1, j] = RectangleSourcesCadre[5];
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            for (int j = 0; j < NBLignes; j++)
            {
                for (int i = 0; i < NBColonnes; i++) 
                {
                    GestionSprites.Draw(TextureCadre, Positions[i, j], PtsTile[i, j], Color.White);
                }
            }
            GestionSprites.End();
            base.Draw(gameTime);
        }
    }
}