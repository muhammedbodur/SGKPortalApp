
<?php

	include ("../baglanti.php");
	
	$sql= "select * from kullanici";
	$tum =mysql_query($sql);
	while($alan=mysql_fetch_array($tum))
	{
	$id=$alan['id'];
	$ryol = $alan['resim_yolu'];
	$update ="update kullanici set resim_yolu ='$id' where resim_yolu ='$ryol'";
	echo $update."<br>";
	mysql_query($update);
	}

?>