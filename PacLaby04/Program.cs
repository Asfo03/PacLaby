using System.Runtime.CompilerServices;
using static Progam;

class Progam
{
	static void Main()
	{
		int ToursJoues = 0;
		int diff = NiveauDifficulte();

		int x = Taille(); 
		int y = 4*x;                                                         //Surface de la 'Cage', représenté par l'axe y et x
		int nbEnnemis = diff * 5;

		Case[,] Cage = new Case[x, y];                                                  //Création de la 'Cage'
		Entite Joueur = new                                                             //Création du 'Joueur'
			(Cage.GetLength(0) / 2, Cage.GetLength(1) / 2, 0, 6 / (diff + 1));          //Emplacement initial au milieu de la Carte
		Entite BaseEnnemie = new Entite(1, Cage.GetLength(1) / 2, 0, nbEnnemis);        //Création du départ 'Ennemi'

		Entite[] Ennemis = new Entite[nbEnnemis];
		for (int numero = 0; numero < Ennemis.Length; numero++)
			Ennemis[numero] = new Entite(1, Cage.GetLength(1) / 2, 0, 0);

		GenerationCage(diff, Cage, Joueur, BaseEnnemie);                              //Permet de générer la Carte


		while (!Victoire(Cage, Joueur, diff) && !Defaite(Joueur))                        //Condition de Victoire
		{                                                                               //puisque lors de l'affichage le score dans la matrice est multiplié par 100
			///Console.Clear();
			Affichage(Cage, Joueur, Ennemis, nbEnnemis, ToursJoues);
			Joueur = MouvementJoueur(Cage, Joueur);

			if (PerteVieJoueur(Cage, Joueur, Ennemis, nbEnnemis))
			{
				Joueur.PV += -1;
				Joueur = Reinitialisation(Cage, Joueur);
				for (int numero = 0; numero < nbEnnemis; numero++)
					Ennemis[numero] = Reinitialisation(Cage, Ennemis[numero]);
				BaseEnnemie.PV = nbEnnemis;
			}
			else
			{
				Ennemis = EnnemiMange(Cage, Ennemis, Joueur, nbEnnemis);
				Cage[Joueur.X, Joueur.Y].Etat = "Joueur";
			}

			for (int mouvEnnemis = 0; mouvEnnemis < diff; mouvEnnemis++)
			{
				if (!Defaite(Joueur))
				{
					Ennemis = MouvementEnnemis(Cage, Ennemis, nbEnnemis);
					Affichage(Cage, Joueur, Ennemis, nbEnnemis, ToursJoues);
				}

				if (PerteVieJoueur(Cage, Joueur, Ennemis, nbEnnemis))
				{
					Joueur.PV += -1;
					Joueur = Reinitialisation(Cage, Joueur);
					for (int numero = 0; numero < nbEnnemis; numero++)
						Ennemis[numero] = Reinitialisation(Cage, Ennemis[numero]);
					BaseEnnemie.PV = nbEnnemis;
				}
				else
				{
					Ennemis = EnnemiMange(Cage, Ennemis, Joueur, nbEnnemis);
					Cage[Joueur.X, Joueur.Y].Etat = "Joueur";
				}
			}

			Vieillissement(Cage);                                                       //Fait Vieillir les murs

			ApparitionEnnemis(Cage, BaseEnnemie, Ennemis, nbEnnemis);					//Soustrait une vie à la BaseEnnemie et propulse un Ennemi dans la Cage

			ToursJoues++;
		}

		Affichage(Cage, Joueur, Ennemis, nbEnnemis, ToursJoues);
		if (Defaite(Joueur))
			Console.WriteLine($"Dommage, votre Score est de : {Cage[Joueur.X, Joueur.Y].Score * 100}");
		else
			Console.WriteLine("Bravo ! Vous avez vaincu PacLaby !");
		Console.ReadLine();
	}

	static int SaisiSecuEntier()
	{
		int entier = 0;
		while (!int.TryParse(Console.ReadLine(), out entier) && entier <= 0)
		{
			Console.WriteLine("Veuillez saisir un entier strictement positif : ");
		}
		return entier;
	}

	static int NiveauDifficulte()
	{
		int diff = 0;
		while (diff < 1 || diff > 3)
		{
			Console.Clear();
			Console.WriteLine("Veuillez choisir votre niveau de difficulté :\n\t1·Facile\n\t2·Normal\n\t3·Difficile");
			diff = SaisiSecuEntier();
		}

		return diff;
	}

	static int Taille()
	{
		int taille = -1;

		while (taille < 0 || taille > 3)
		{
			Console.Clear();
			Console.WriteLine("Veuillez choisir votre taille de Carte :\n\t0·Personnalisée\n\t1·Petite\n\t2·Normal\n\t3·Grande");
			taille = SaisiSecuEntier();
		}

		switch (taille)
		{
			case 0:
				while (taille < 12 || taille > 36)
				{
					Console.Clear();
					Console.WriteLine("Veuillez saisir votre taille de Carte (compris entre 12 et 36) :");
					taille = SaisiSecuEntier();
				}
				break;
			default: taille = 8 + taille*4; break;
		}

		return taille;
	}

	static void GenerationCage(int diff, Case[,] Cage, Entite J, Entite BaseEnnemie)
	{
		if (Cage != null && Cage.GetLength(0) > 0 && Cage.GetLength(1) > 0)
		{
			int var = 0;
			int decompte = 0;

			int nbMurs = 0;
			int cerise = 0;
			int pacGomme = 0;
			int x = Cage.GetLength(0);

			switch (diff)                                                               //S'adapte en fonction de la Difficulté choisie
			{
				case 1:
					nbMurs = 6 * x;
					cerise = Convert.ToInt32(1.25*x);
					pacGomme = (2*x)/3;
					break;

				case 2:
					nbMurs = 8 * x;
					cerise = x;
					pacGomme = x/2;
					break;

				case 3:
					nbMurs = 10 * x;
					cerise = Convert.ToInt32(0.75*x);
					pacGomme = x/3;
					break;
			}

			PacGomme.Nombre = pacGomme;

			Random aleatoire = new Random();
			for (int boucle = 0; boucle < 6; boucle++)
				for (int i = 0; i < Cage.GetLength(0); i++)
					for (int j = 0; j < Cage.GetLength(1); j++)
					{
						if (boucle == 4 && decompte < nbMurs) boucle--;
						if (boucle == 5 && (cerise > 0 || pacGomme > 0)) boucle--;

						switch (boucle)
						{
							case 0:                                                     //les murs universel sont mis à '-1'
								if (i % 2 == 0 && j % 2 == 0)                               //ces 'Mur' sont des pilliers qui ne subissent pas le vieillissement
								{
									Cage[i, j].Etat = "Mur";                            //Ils sont nommés comme tous les 'Mur' afin de garder le meme affichage
									Cage[i, j].Duree = -1;                              //Tout emplacement avec une 'Duree' < 0 ne vieillit pas
								}
								break;

							case 1:                                                     //On définit l'ensemble du terrain
								if (i == J.X && j == J.Y)                               //on localise l'emplacement 'Joueur'
								{
									Cage[i, j].Etat = "Joueur";                         //on identifie l'emplacement
									Cage[i, j].Duree = -1;
								}
								else if (i == BaseEnnemie.X && j == BaseEnnemie.Y)                          //
								{
									Cage[i, j].Etat = "BaseEnnemie";
									Cage[i, j].Duree = -1;
								}
								break;

							case 2:                                                     //Crée une zone libre autour du Départ Joueur
								if (i + 2 > J.X && i - 2 < J.X && j + 2 > J.Y && j - 2 < J.Y && Cage[i, j].Etat != "Joueur")
								{                                                       //On définit une aura autour du 'Joueur' afin qu'il puisse se mouvoir dès le début
									Cage[i, j].Etat = "Libre";
									Cage[i, j].Duree = -1;                              //'-1' parce qu'on ne veut pas de score
								}

								if (i + 2 > BaseEnnemie.X && i - 2 < BaseEnnemie.X && j + 2 > BaseEnnemie.Y && j - 2 < BaseEnnemie.Y && Cage[i, j].Etat != "BaseEnnemie")
								{                                                       //Crée une zone libre autour de BaseEnnemie
									Cage[i, j].Etat = "Libre";
									Cage[i, j].Duree = -1;
								}
								break;

							case 3:
								if ((i % 2 == 0 || j % 2 == 0) && Cage[i, j].Duree == 0 && decompte < nbMurs)
								{                                                       //permet de créer des murs pour former des chemins sur les cases autorisées
									var = aleatoire.Next(Cage.GetLength(1));

									if (var == 1)
									{
										Cage[i, j].Etat = "Mur";

										var = aleatoire.Next(2, 7);
										Cage[i, j].Duree = var;
										decompte++;                                     //On veut maitriser combien de murs mobiles sont présents
									}
									else Cage[i, j].Etat = "Libre";
								}
								else if (Cage[i, j].Duree == 0) Cage[i, j].Etat = "Libre";
								break;

							case 4:                                                     //On génére une score à toutes les cases accessibles pour le Joueur
								if (Cage[i, j].Duree >= 0)
								{
									if (Cage[i, j].Score == 0) Cage[i, j].Score = 1;
									if (i % 2 != 0 && j % 2 != 0)                           //On génére les cérises qui vont aux emplacements sans murs
									{
										var = aleatoire.Next(Cage.GetLength(1));
										if (var == 1 && cerise > 0)
										{
											Cage[i, j].Score = 5; cerise--;
										}
										else if (var == 2 && pacGomme > 0)
										{
											Cage[i, j].Score = 2; pacGomme--;
										}
									}
								}
								break;
						}
					}
		}
	}

	static Entite Reinitialisation(Case[,] Cage, Entite Ent)
	{
		if (Ent.PV > 0)
		{
			int i = Ent.X, j = Ent.Y;

			Ent.X = Cage.GetLength(0) / 2;
			Ent.Y = Cage.GetLength(1) / 2;
			Ent.Or = 0;

			Cage[Ent.X, Ent.Y].Score = Cage[i, j].Score;
			Cage[i, j].Score = 0;
			Cage[i, j].Etat = "Libre";

			Cage[Ent.X, Ent.Y].Etat = "Joueur";
		}
		else
		{
			for (int i = 0; i < Cage.GetLength(0); i++)
				for (int j = 0; j < Cage.GetLength(1); j++)
					if (Cage[i, j].Etat == "Ennemi") Cage[i, j].Etat = "Libre";

			Ent.X = 1;
			Ent.Y = Cage.GetLength(1) / 2;
			Ent.Or = 0;
		}
		return Ent;
	}

	static Entite[] EnnemiMange(Case[,] Cage, Entite[] E, Entite J, int nbEnnemis)
	{
		for (int numero = 0; numero < nbEnnemis; numero++)
			if (J.X == E[numero].X && J.Y == E[numero].Y)
			{
				E[numero].X = 1;
				E[numero].Y = Cage.GetLength(1) / 2;
				E[numero].Or = 0;
			}

		return E;
	}

	static Entite ApparitionEnnemis(Case[,] Cage, Entite BaseEnnemie, Entite[] E, int nbEnnemis)
	{
		bool spawnOccupe = false;
		int lancement = -1;

		for (int numero = 0; numero < nbEnnemis; numero++)
		{
			if (BaseEnnemie.X + 1 == E[numero].X && BaseEnnemie.Y == E[numero].Y)
				spawnOccupe = true;
			if (E[nbEnnemis - 1 - numero].X == BaseEnnemie.X && E[nbEnnemis - 1 - numero].Y == BaseEnnemie.Y)
				lancement = nbEnnemis - 1 - numero;
		}

		if (BaseEnnemie.PV > 0 && !spawnOccupe && lancement >= 0)
		{
			Cage[BaseEnnemie.X + 1, BaseEnnemie.Y].Etat = "Ennemi";
			E[lancement].X = BaseEnnemie.X + 1; E[lancement].Y = BaseEnnemie.Y;
			BaseEnnemie.PV--;
		}

		return BaseEnnemie;
	}

	static Entite MouvementJoueur(Case[,] Cage, Entite J)
	{
		string mouv;
		do
		{
			Console.Write("Saisi mouvement : ");									//On informe l'Utilisateur
			mouv = Convert.ToString(Console.ReadKey().Key);						//Modification du mouv pour interprétation
			Console.WriteLine();														//Ajout purement esthétique
		}
		while (mouv != null && mouv != "Z" && mouv != "Q" && mouv != "S" && mouv != "D" && mouv != "P");
																						//"P" est la touche attribuée pour 'pause' afin que le joueur puisse décider de ne pas bouger
																						//Si 'mouv' n'est pas un de ces cas, 'mouv' n'est pas interprétable donc on réeffectue la saisie
		return Deplacement(Cage, J, mouv);											//les Coordonnées 'J' sont retournées parce que ce n'est pas comme un tableau
	}

	static Entite[] MouvementEnnemis(Case[,] Cage, Entite[] E, int nbEnnemis)
	{
		for (int numero = 0; numero < nbEnnemis; numero++)
		{
			if (Cage[E[numero].X, E[numero].Y].Etat != "BaseEnnemie")
				E[numero] = MouvementEnnemisRandom(Cage, E[numero]);
		}

		return E;
	}

	static Entite MouvementEnnemisRandom(Case[,] Cage, Entite E)
	{
		int[] compteur;

		string mouv = "0";
		compteur = TestDeplacement(Cage, E);

		Random ran = new Random();
		if (compteur[0] + compteur[1] + compteur[2] + compteur[3] < 4)
		{
			do
			{
				switch (ran.Next(5))
				{
					case 0: mouv = " "; break;
					case 1:
						if (compteur[0] == 0)
							mouv = "Z";
						else mouv = "0";
						break;
					case 2:
						if (compteur[1] == 0)
							mouv = "Q";
						else mouv = "0";
						break;
					case 3:
						if (compteur[2] == 0)
							mouv = "S";
						else mouv = "0";
						break;
					case 4:
						if (compteur[3] == 0)
							mouv = "D";
						else mouv = "0";
						break;
				}
			}
			while (mouv == "0");
			E = Deplacement(Cage, E, mouv);
		}
		return E;
	}

	static int[] TestDeplacement(Case[,] Cage, Entite Ent)
	{
		int[] compteur = { 0, 0, 0, 0 };

		Entite Sauvegarde = Ent;

		for (int i = 0; i < 4; i++)
		{
			switch (i)                                                               //interprétation du contenu de 'mouv'
			{
				case 0: Ent.X = Ent.X - 1; break;
				case 1: Ent.Y = Ent.Y - 1; break;
				case 2: Ent.X = Ent.X + 1; break;
				case 3: Ent.Y = Ent.Y + 1; break;
			}

			//Transforme la matrice en matrice circulaire
			if (Ent.X < 0) Ent.X = Cage.GetLength(0) - 1;
			if (Ent.Y < 0) Ent.Y = Cage.GetLength(1) - 1;
			if (Ent.X > Cage.GetLength(0) - 1) Ent.X = 0;
			if (Ent.Y > Cage.GetLength(1) - 1) Ent.Y = 0;

			if (Cage[Ent.X, Ent.Y].Etat == "Mur" || Cage[Ent.X, Ent.Y].Etat == "BaseEnnemie" || Cage[Ent.X, Ent.Y].Etat == "Ennemi")
				compteur[i] = 1;

			Ent = Sauvegarde;
		}

		return compteur;
	}

	static Entite Deplacement(Case[,] Cage, Entite Ent, string mouv)                    //'Deplacement' peut déplacer toute entité représenter par la struct 'Coordonnee'
	{
		if (Cage.GetLength(0) > 0 && Cage.GetLength(0) > 0)
		{
			string memoire = Cage[Ent.X, Ent.Y].Etat;
			Cage[Ent.X, Ent.Y].Etat = "enDeplacement";

			switch (mouv)                                                               //interprétation du contenu de 'mouv'
			{
				case "Z": Ent.X = Ent.X - 1; break;
				case "Q": Ent.Y = Ent.Y - 1; break;
				case "S": Ent.X = Ent.X + 1; break;
				case "D": Ent.Y = Ent.Y + 1; break;
			}
			//Transforme la matrice en matrice circulaire
			if (Ent.X < 0) Ent.X = Cage.GetLength(0) - 1;
			if (Ent.Y < 0) Ent.Y = Cage.GetLength(1) - 1;
			if (Ent.X > Cage.GetLength(0) - 1) Ent.X = 0;
			if (Ent.Y > Cage.GetLength(1) - 1) Ent.Y = 0;
			//Si le déplacement se finit sur un 'Mur' ou sur 'BaseEnnmie', il est annulé
			if (Cage[Ent.X, Ent.Y].Etat == "Mur" || Cage[Ent.X, Ent.Y].Etat == "BaseEnnemie")
			{                                                                           //le déplacement est annulé et on effectue le mouvement inverse																						
				switch (mouv)                                                           //interprétation de 'mouv' pour savoir ou revenir en arrière
				{
					case "Z": Ent.X = Ent.X + 1; break;
					case "Q": Ent.Y = Ent.Y + 1; break;
					case "S": Ent.X = Ent.X - 1; break;
					case "D": Ent.Y = Ent.Y - 1; break;
				}
				//Transforme la matrice en matrice circulaire
				if (Ent.X < 0) Ent.X = Cage.GetLength(0) - 1;
				if (Ent.Y < 0) Ent.Y = Cage.GetLength(1) - 1;
				if (Ent.X > Cage.GetLength(0) - 1) Ent.X = 0;
				if (Ent.Y > Cage.GetLength(1) - 1) Ent.Y = 0;
			}

			for (int i = 0; i < Cage.GetLength(0); i++)
				for (int j = 0; j < Cage.GetLength(1); j++)
					if (Cage[i, j].Etat == "enDeplacement" && (Ent.X != i || Ent.Y != j))   //On vérifie que le joueur se soit déplacé
					{
						Cage[i, j].Etat = "Libre";
						if (memoire == "Joueur")
						{
							if (Cage[Ent.X, Ent.Y].Score == 2)
							{
								Cage[Ent.X, Ent.Y].Score -= 2;
								PacGomme.Temps = 10;
								PacGomme.Nombre--;
								PacGomme.FantomeMangee = 0;
							}
							Cage[Ent.X, Ent.Y].Score += Cage[i, j].Score;               //On additionne le score du joueur avec le score de sa case d'arrivée
							Cage[i, j].Score = 0;                                       //L'ancien score joueur qui était présent est effacé
						}
					}

			switch (mouv)                                                               //On convertit le mouvement en chiffre, il peut alors etre stocker dans 'Duree'
			{
				case "Z": mouv = "1"; break;
				case "Q": mouv = "2"; break;
				case "S": mouv = "3"; break;
				case "D": mouv = "4"; break;
				default: mouv = "0"; break;
			}
			Ent.Or = Convert.ToInt32(mouv);										//'Duree' n'est pas utile pour Entité, il est donc utilisé pour communiquer l'orientation
			Cage[Ent.X, Ent.Y].Etat = memoire;                                          //Peu importe le nouveau emplacement de l'Entité, il est mis à son état d'origine
		}
		return Ent;
	}

	static void Affichage(Case[,] Cage, Entite J, Entite[] E, int nbEnnemis, int Tours)
	{
		//
		AffichageEntite(Cage, J);
		for (int numero = 0; numero < nbEnnemis; numero++)
			AffichageEntite(Cage, E[numero]);
		//

		if (Cage != null && Cage.GetLength(0) > 0 && Cage.GetLength(1) > 0)
		{
			for (int i = 0; i < Cage.GetLength(0); i++)
			{
				for (int j = 0; j < Cage.GetLength(1); j++)
					switch (Cage[i, j].Etat)
					{
						case "Mur":                                                     //La couleur des Murs va etre différente en fonction de la durée de vie restante
							switch (Cage[i, j].Duree)
							{
								case < 0: Console.ForegroundColor = ConsoleColor.DarkGreen; break;  //Le mur avec une durée négative ne sont pas impactés par le vieillissement
								case 1: Console.ForegroundColor = ConsoleColor.DarkRed; break;
								case 2: Console.ForegroundColor = ConsoleColor.Red; break;
								case 3: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
								case > 5: Console.ForegroundColor = ConsoleColor.DarkGreen; break;
								case > 3: Console.ForegroundColor = ConsoleColor.Green; break;
							}
							Console.Write("█");
							Console.ResetColor();
							break;

						case "Libre":
							switch (Cage[i, j].Score)
							{
								case 0: Console.Write(" "); break;
								case 1: Console.Write("·"); break;
								case 2: Console.Write("*"); break;
								case 5: Console.Write("%"); break;                 //Les cerises sont des bonus de Score
							}
							break;

						case "Joueur":                                                  //Le joueur est affiché en Bleu et en fonction de son mouvement précédent
							Console.ForegroundColor = ConsoleColor.Blue;
							switch (J.Or)                                               //'Or' est l'indication d'orientation pour toutes les entités
							{
								case 0: Console.Write("O"); break;
								case 1: Console.Write("^"); break;
								case 2: Console.Write("<"); break;
								case 3: Console.Write("v"); break;
								case 4: Console.Write(">"); break;
							}
							Console.ResetColor();
							break;

						case "Ennemi":                                                  //L'ennemi est affiché en Bleu et en fonction de son mouvement précédent
							Console.ForegroundColor = ConsoleColor.Red;

							int numero = 0;
							while (numero < nbEnnemis)
							{
								if (E[numero].X == i && E[numero].Y == j) break;
								numero++;
							}

							switch (E[numero].Or)                                   //'Or' est l'indication d'orientation pour toutes les entités
							{
								case 0: Console.Write("O"); break;
								case 1: Console.Write("^"); break;
								case 2: Console.Write("<"); break;
								case 3: Console.Write("v"); break;
								case 4: Console.Write(">"); break;
							}

							Console.ResetColor();
							break;

						case "BaseEnnemie":
							Console.ForegroundColor = ConsoleColor.DarkRed;
							switch (Cage[i, j].Duree)
							{
								case -1: Console.Write("×"); break;
								case 1: Console.Write("+"); break;
							}
							Console.ResetColor();
							break;

						default:                                                        //Utile lors des tests ou afin de détecter des erreurs potentiels
							Console.ForegroundColor = ConsoleColor.Black;
							Console.BackgroundColor = ConsoleColor.Yellow;
							Console.Write("!");
							Console.ResetColor();
							break;
					}
				Console.WriteLine();
			}
			//Points de Vie Joueur
			Console.Write("PV : ");
			Console.ForegroundColor = ConsoleColor.Red;
			for (int Vies = 0; Vies < J.PV; Vies++)
				Console.Write("♥");
			Console.WriteLine();
			Console.ResetColor();

			//Décompte Score
			Console.WriteLine($"Score : {Cage[J.X, J.Y].Score * 100}");         //le stockage du score est dans la 'Case.Score' 'Joueur'
			Console.WriteLine($"Tour n°{Tours}");

			//Affichage du temps restant pour la pacgomme
			for (int i = 0; i < PacGomme.Temps; i++)
			{
				Console.Write("/");
			}
			Console.WriteLine();
		}
	}

	static void AffichageEntite(Case[,] Cage, Entite Ex)
	{
		Console.WriteLine($"{Ex.X} ; {Ex.Y} ; {Ex.Or} ; {Ex.PV} : {Cage[Ex.X, Ex.Y].Etat}");
	}

	static void Vieillissement(Case[,] Cage)
	{
		int decompte = 0;
		if (PacGomme.Temps != 0)
		{
			PacGomme.Temps--;
		}
		for (int boucle = 0; boucle < 2; boucle++)                                      //Cette boucle 'for' est adaptable au nombre de 'case' et permet de ne pas répéter le double 'for'
			for (int i = 0; i < Cage.GetLength(0); i++)
				for (int j = 0; j < Cage.GetLength(1); j++)
					switch (boucle)
					{
						case 0:                                                         //Diminue la duree de tous les Murs avec une durée positive
							if (Cage[i, j].Etat == "Mur" && Cage[i, j].Duree > 0)       //Les cases 'Mur' d'une durée < 0 ne vieillissent pas
								Cage[i, j].Duree--;
							break;
						case 1:                                                         //Transforme tous les murs tombés à '0' en case vide
							if (Cage[i, j].Etat == "Mur" && Cage[i, j].Duree == 0)
							{
								Cage[i, j].Etat = "Libre";
								decompte++;                                             //Sert à maintenir le nombre de murs sur la carte
							}
							break;
					}


		Console.WriteLine(decompte);                                                //Affichage le nombre de murs bougeant de places ce tour-ci
		int var = 0;
		while (decompte > 0)                                                            //Tant que le nombre de murs enlevés n'est pas remis sur le plateau, la boucle continue
			for (int i = 0; i < Cage.GetLength(0); i++)
				for (int j = 0; j < Cage.GetLength(1); j++)
				{
					Random aleatoire = new Random();
					var = aleatoire.Next(1, 100);                       //La probabilité d'apparition est faible pour ne pas concentrer tous les murs en haut de la Carte

					if (i % 2 == 0 || j % 2 == 0)                                           //les emplacements pour nouveaux murs afin de concerver un aspect chemin
						if (Cage[i, j].Etat == "Libre" && Cage[i, j].Duree == 0 && decompte > 0)
							if (var > 3 && var < 7)                                     //Tous les 'if' auraient pu etre regrouper mais il aurait été difficile de commenter
							{
								Cage[i, j].Etat = "Mur";
								Cage[i, j].Duree = var;                                 //Cette donnée est totalement arbitraire et peut etre changée en fonction de la difficulté
								decompte--;
							}
				}
	}

	static bool PerteVieJoueur(Case[,] Cage, Entite J, Entite[] E, int nbEnnemis)
	{
		bool perte = false;
		for (int numero = 0; numero < nbEnnemis; numero++)
			if (PacGomme.Temps == 0 && J.X == E[numero].X && J.Y == E[numero].Y)
				perte = true;
			else if (J.X == E[numero].X && J.Y == E[numero].Y)
			{
				PacGomme.FantomeMangee++;
				Cage[J.X, J.Y].Score += 5 * PacGomme.FantomeMangee;
			}

		return perte;
	}

	static bool PerteVieEnnemi()
	{
		return false;
	}

	static bool Victoire(Case[,] Cage, Entite J, int diff)
	{
		return Cage[J.X, J.Y].Score >= 25 + 25*diff;
	}

	static bool Defaite(Entite J)
	{
		return J.PV <= 0;
	}

	public struct Case
	{
		private string etat;                                                            //Etat stocke le type de case
		private int parametreEtat;                                                      //Précise l'Etat
		private int score;                                                              //
		private int duree;                                                              //Duree stocke la durée mais aussi la "sacralité" des cases si elles sont négatives

		public string Etat
		{ get { return etat; } set { etat = value; } }

		public int ParaEtat
		{ get { return parametreEtat; } set { parametreEtat = value; } }

		public int Score
		{ get { return score; } set { score = value; } }

		public int Duree
		{ get { return duree; } set { duree = value; } }
	}

	public struct Entite
	{
		private int axeX;
		private int axeY;
		private int orientation;
		private int pointDeVie;

		public Entite(int x, int y, int or, int pv)
		{
			this.axeX = x;
			this.axeY = y;
			this.orientation = or;
			this.pointDeVie = pv;
		}

		public int X
		{ get { return axeX; } set { axeX = value; } }

		public int Y
		{ get { return axeY; } set { axeY = value; } }

		public int Or
		{ get { return orientation; } set { orientation = value; } }

		public int PV
		{ get { return pointDeVie; } set { pointDeVie = value; } }
	}

	static class PacGomme
	{                                                                                   //Permet de créer une variable accessible partout
		public static int Nombre;
		public static int Temps;
		public static int FantomeMangee;
	}
}
