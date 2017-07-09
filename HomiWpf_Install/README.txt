Comment compiler un package d'installation ?
--------------------------------------------

Il suffit d'executer le script "make_full.bat"
Il s'agit d'un script "batch" intéractif.

Il vous sera demandé le numéro de version composé de 3 parties : 
* Le numéro de Version correspond à la version "majeure". Ex. 1.0, 1.1, 1.2, etc. Il doit être incrémenté à chaque évolution majeure (ajout fonctionnel, ensemble de correctifs, ..)
* Le numéro de Build correspond à version "mineure", Ex. 1.0.1, 1.0.2, 1.0.3, etc. Il doit être incrémenté à chaque création d'un nouveau package. 
* Le numéro de Revision correspond à la révision SVN courante. Ex. 1.0.1.852, 1.1.5.1745, etc.
* La version "Stable" ou "Release"
Les packages sont générés dans le dosseir ~/HoMIDoM_Install/packages

NSIS (Nullsoft Scriptable Install System) est un système de création de package d'installation à partir de scripts
=> http://nsis.sourceforge.net/Main_Page

Pour compiler un script NSIS vous devez disposer des binaires NSIS. ils sont intégrés aux source dans le dossier ~/HoMIDoM_Install/tools/nsis-2.46.
