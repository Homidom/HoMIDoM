Comment compiler un package d'installation ?
--------------------------------------------

Il suffit d'executer le script "make_full.bat"
Il s'agit d'un script "batch" intéractif.

Il vous sera demandé le numéro de version composé de 3 parties : 
* Le numéro de Version correspond à la version "majeure". Ex. 1.0, 1.1, 1.2, etc. Il doit être incrémenté à chaque évolution majeure (ajout fonctionnel, ensemble de correctifs, ..)
* Le numéro de Build correspond à version "mineure", Ex. 1.0.1, 1.0.2, 1.0.3, etc. Il doit être incrémenté à chaque création d'un nouveau package. 
* Le numéro de Revision correspond à la révision SVN courante. Ex. 1.0.1.852, 1.1.5.1745, etc.

Les packages sont générés dans le dosseir ~/HoMIDoM_Install/packages

NSIS (Nullsoft Scriptable Install System) est un système de création de package d'installation à partir de scripts
=> http://nsis.sourceforge.net/Main_Page

Pour compiler un script NSIS vous devez disposer des binaires NSIS. ils sont intégrés aux source dans le dossier ~/HoMIDoM_Install/tools/nsis-2.46.

Comment ajouter un nouveau Driver ?
-----------------------------------

L'inclusion des Drivers dans les packages d'installation se fait via des fichiers ".nsh" présent dans le dossier ~/HoMIDoM_Install/includes.
Le script NSIS principale inclus l'ensembles des fichiers "Driver_*.nsh".
Il suffit de copier/coller un fichier ".nsh" existant et le modifier en fonction du Driver à inclure.

Ex. fichier "Driver_KNX.nsh" 

  Section "KNX/EIBD" DRIVER_KNX 	=> Début de la section du driver : changer le libellée (entre "") et le nom de la section (DRIVER_KNX)
    SectionIn 1 2    				=> Indique à NSIS que cette section fait partie du profil d'installation 1 (complete) et 2 (service)
    CreateDirectory "$INSTDIR\Drivers\KNX"  => création de(s) dossiers supplémentaires spécifiques au driver (optionnel)
    SetOutPath "$INSTDIR\Drivers" => indique à NSIS où les fichiers doivent être déployée (ne pas changer)
    File "..\RELEASE\Drivers\Driver_KNX.dll"  => indique à NSIS où il doit prendre le(s) fichier(s) à inclure dans le package (ici, la dll du driver. pour rajouter d'autres fichiers, dupliquer la ligne.)
    ${If} ${RunningX64} => inclusion de(s) fichier(s) supplémentaire nécessaires au fonctionnement du driver. en fonction de l'OS cible (32bit/64bit)
      File  "..\Dll_externes\Homidom-64bits\Drivers\KNX*.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\KNX*.dll"
    ${EndIf}
  SectionEnd => fin de la section


