<?php
	if(isset($_SESSION['user_id'])){ 
?>
<div class="main">
 <table border="0" width="100%">
  <tbody>
    <tr>
      <td align="center" valign="middle">
        <table border="0" width="100%">
    
<?php
include("./include_php/config.php");

echo "<tr height=20><td colspan=5 align=center><div class=\"plan\" id=\"plan\">";
?>

<script type='text/javascript'>
var IP='http://192.168.1.5';
var A=window.location.search;A=A.substring(1,5);if(A=="")A=1;
NewImage=new Image();var I=new Date().getTime();var imgW=640*A;
function ImgRefresh(){document.getElementById("imgDisplay").src=IP+'/videostream.cgi';}
document.write("<script type='text/javascript' src='"+IP+"/get_camera_params.cgi?NEXT_URL=camera_params.js'><\/script>");
function OnParam(){var C=document.getElementById("cache").style;if(C.display=='')C.display='none';else C.display='';}
function OnResol(){if(resolution==8)resolution=32;else resolution=8;document.forms["R"].Resolution.value=resolution;Reglage(0,resolution);}
function OnPtzMouse(M,X){NewImage.src=IP+'/decoder_control.cgi?command='+M+'&onestep'+X+'&'+I;}
function OnZoom(){
 if(imgW==640*A)document.getElementById("zoom").className='normal';else document.getElementById("zoom").className='rognage';
 imgW=document.getElementById("imgDisplay").width;imgH=document.getElementById("imgDisplay").height;
 document.getElementById("mapParam").coords="0,0,"+(imgW/3-10)+","+(imgH/3-10);
 document.getElementById("mapLeft").coords="0,"+(imgH/3)+",20,"+(imgH/3*2);
 document.getElementById("mapLeft1").coords="25,"+(imgH/3)+","+(imgW/3)+","+(imgH/3*2);
 document.getElementById("mapRight1").coords=(imgW/3*2)+","+(imgH/3)+","+(imgW-25)+","+(imgH/3*2);
 document.getElementById("mapRight").coords=(imgW-20)+","+(imgH/3)+","+imgW+","+(imgH/3*2);
 document.getElementById("mapZoom").coords=(imgW/3+10)+","+(imgH/3+10)+","+(imgW/3*2-10)+","+(imgH/3*2-10);
 document.getElementById("mapUp").coords=(imgW/3)+",0,"+(imgW/3*2)+",20";
 document.getElementById("mapUp1").coords=(imgW/3)+",25,"+(imgW/3*2)+","+(imgH/3);
 document.getElementById("mapDown1").coords=(imgW/3)+","+(imgH/3*2)+","+(imgW/3*2)+","+(imgH-25);
 document.getElementById("mapDown").coords=(imgW/3)+","+(imgH-20)+","+(imgW/3*2)+","+imgH;
 document.getElementById("mapResol").coords=(imgW/3*2+10)+",0,"+imgW+","+(imgH/3-10);
 document.getElementById("mapLumM").coords="0,"+(imgH/3*2+10)+","+(imgW/3-10)+","+imgH;
 document.getElementById("mapLumP").coords=(imgW/3*2+10)+","+(imgH/3*2+10)+","+imgW+","+imgH;
}
function FlipLRUD(F){if(F==3){fL=4;fR=6;fU=2;fD=0;}else if(F==2){fL=4;fR=6;fU=0;fD=2;}else if(F==1){fL=6;fR=4;fU=2;fD=0;}else{fL=6;fR=4;fU=0;fD=2;}}
function OnLed(L){NewImage.src=IP+'/set_misc.cgi?led_mode='+L+'&'+I;}
function Reglage(P,V){NewImage.src=IP+'/camera_control.cgi?param='+P+'&value='+V+'&'+I;}
function OnLum(L){brightness+=L;document.forms["R"].Brightness.value=brightness/16;Reglage(1,brightness);}
function OnCont(L){contrast+=L;document.forms["R"].Contraste.value=contrast;Reglage(2,contrast);}
function func_KeyPress(event){
 var TouchKeyPress=(window.Event)?event.which:event.keyPress;
 switch(TouchKeyPress){
 case 97:OnParam();break;
 case 81:OnPtzMouse(fL);break;
 case 113:OnPtzMouse(fL,'=1');break;
 case 100:OnPtzMouse(fR,'=1');break;
 case 68:OnPtzMouse(fR);break;
 case 115:OnZoom();break;
 case 90:OnPtzMouse(fU);break;
 case 122:OnPtzMouse(fU,'=1');break;
 case 120:OnPtzMouse(fD,'=1');break;
 case 88:OnPtzMouse(fD);break;
 case 101:OnResol();break;
 case 119:OnLum(-16);break;
 case 99:OnLum(+16);break;
 case 87:OnCont(-1);break;
 case 67:OnCont(+1);break;}
}

/*
document.write("<style type='text/css'> body {padding:0;margin:0;}");
document.write(".rognage {width:"+320*A+"px;height:"+240*A+"px;overflow:hidden;position:relative;}");
document.write(".rognage img{width:"+640*A+"px;height:"+480*A+"px;margin-left:-"+160*A+"px;margin-top:-"+120*A+"px;}");
document.write(".normal {width:"+320*A+"px;height:"+240*A+"px;position:relative;}");
document.write(".normal img{width:"+320*A+"px;height:"+240*A+"px;margin-left:0;margin-top:0;}</style>");
*/

</script>
<!--</head>
<body onload=ImgRefresh() onkeypress=func_KeyPress(event)>-->
 <table cellspacing=0 cellpadding=0><tr><td>
 <div id=zoom>
  <img  alt="cam1" id=imgDisplay usemap="#map" border=0 width=640 height=480 >
 </div>
 <map name=map>
  <area id=mapParam  shape=rect href=# onclick=OnParam() title='Param&eacute;trage'>

  <area id=mapLeft   shape=rect href=# onclick=OnPtzMouse(fL) title='<<'>
  <area id=mapLeft1  shape=rect href=# onclick=OnPtzMouse(fL,'=1') title='<' ondblclick=OnPtzMouse(fL)>
  <area id=mapRight1 shape=rect href=# onclick=OnPtzMouse(fR,'=1') title='>' ondblclick=OnPtzMouse(fR)>
  <area id=mapRight  shape=rect href=# onclick=OnPtzMouse(fR) title='>>'>
  <area id=mapZoom   shape=rect href=# onclick=OnZoom() title='Zoom'>
  <area id=mapUp     shape=rect href=# onclick=OnPtzMouse(fU) title='^^'>
  <area id=mapUp1    shape=rect href=# onclick=OnPtzMouse(fU,'=1') title='^' ondblclick=OnPtzMouse(fU)>
  <area id=mapDown1  shape=rect href=# onclick=OnPtzMouse(fD,'=1') title='v' ondblclick=OnPtzMouse(fD)>
  <area id=mapDown   shape=rect href=# onclick=OnPtzMouse(fD) title='v v'>

  <area id=mapResol  shape=rect href=# onclick=OnResol()  title='R&eacute;solution'>
  <area id=mapLumM   shape=rect href=# onclick=OnLum(-16) title='Luminosit&eacute; -'>
  <area id=mapLumP   shape=rect href=# onclick=OnLum(+16) title='Luminosit&eacute; +'>
 </map>
 <div align=center id=cache style="display:none;">
  <form name=R><table><tr>
  <td><input type=button value='Switch On'  onclick=OnPtzMouse(94)></td>
  <td><input type=button value='Switch Off' onclick=OnPtzMouse(95)></td>
  <td><input type=button value='Led On'  onclick=OnLed(1)></td>

  <td><input type=button value='Led Off' onclick=OnLed(2)></td>
  </tr></table><table>
  <script type='text/javascript'>
   document.write("<tr><td>R&eacute;solution (8/32)	</td><td><INPUT name=Resolution size=2 value="+resolution+"	 onBlur=\"Reglage(0,Resolution.value)\"></td></tr>");
   document.write("<tr><td>Luminosit&eacute; (0-15)	</td><td><INPUT name=Brightness size=2 value="+brightness/16+" onBlur=\"Reglage(1,Brightness.value*16)\"></td></tr>");
   document.write("<tr><td>Contraste (1-6)		</td><td><INPUT name=Contraste  size=2 value="+contrast+"	 onBlur=\"Reglage(2,Contraste.value)\"></td></tr>");
   document.write("<tr><td>Mode (0-2)			</td><td><INPUT name=Mode	  size=2 value="+mode+"		 onBlur=\"Reglage(3,Mode.value)\"></td></tr>");
   document.write("<tr><td>Flip (0-3)			</td><td><INPUT name=Flip	  size=2 value="+flip+"		 onBlur=\"FlipLRUD(Flip.value);Reglage(5,Flip.value);\"></td></tr>");

  </script></table><a href=Hm.txt>Aide</a></form>
 </div>
 </td></tr></table>
<script type='text/javascript'>
   ImgRefresh();
   OnZoom();
   FlipLRUD(flip);
   /*timeoutID = window.setTimeout("window.ImgRefresh()",1000);*/
</script>
 
<?php
 echo "</div>";
?>

        </table>
      </td>
    </tr>
  </tbody>
 </table>
</div>
<?php
	}else{
		header("location:../index.php");
	}
?>