<?php
$no = $_POST["id"];
exec("cd program && ./run.sh ".$no,$out,$return);
$num=count($out);
echo $out[$num-2]."<br>";
echo $out[$num-1]."<br>";
echo "return code :".$return;
?>