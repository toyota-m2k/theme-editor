# Resolves the standard Material 3 (2021 spec) per-role tone (HCT tone == CIELAB L*)
# for Normal / Medium / High contrast, in Light and Dark modes. Faithful port of
# Material Color Utilities getTone + ContrastCurve + ToneDeltaPair + highestSurface.
#
# The tones are hue-independent, so this output is the source for the baked table in
# ThemeEditor/ThemeEditor/StandardTones.cs. Validation: the Normal column reproduces
# the known base tones exactly (primary 40/80, container 90/30, onSurface 10/90, ...).
# To target a different spec version, adjust the spec tables below (or read L* from a
# pristine Theme Builder export) and paste the result into StandardTones.cs.
#
# Run:  pwsh -NoProfile -File tools/resolve_material_tones.ps1

function yFromLstar($l){ if($l -gt 8.0){100.0*[math]::Pow(($l+16.0)/116.0,3)}else{100.0*$l/903.2962962962963} }
function lstarFromY($y){ $yn=$y/100.0; if($yn -gt 0.008856451679035631){$f=[math]::Cbrt($yn)}else{$f=(903.2962962962963*$yn+16.0)/116.0}; 116.0*$f-16.0 }
function ratioOfYs($a,$b){ $l=[math]::Max($a,$b);$d=[math]::Min($a,$b);($l+5.0)/($d+5.0) }
function ratioOfTones($a,$b){ $a=[math]::Max(0.0,[math]::Min(100.0,$a));$b=[math]::Max(0.0,[math]::Min(100.0,$b)); ratioOfYs (yFromLstar $a) (yFromLstar $b) }
function lighter($tone,$ratio){ $dy=yFromLstar $tone;$ly=$ratio*($dy+5.0)-5.0; if($ly -lt 0 -or $ly -gt 100){return -1}; $rc=ratioOfYs $ly $dy; if(($rc -lt $ratio)-and([math]::Abs($rc-$ratio)-gt 0.04)){return -1}; $rv=(lstarFromY $ly)+0.4; if($rv -lt 0 -or $rv -gt 100){return -1}; $rv }
function darker($tone,$ratio){ $ly=yFromLstar $tone;$dy=(($ly+5.0)/$ratio)-5.0; if($dy -lt 0 -or $dy -gt 100){return -1}; $rc=ratioOfYs $ly $dy; if(($rc -lt $ratio)-and([math]::Abs($rc-$ratio)-gt 0.04)){return -1}; $rv=(lstarFromY $dy)-0.4; if($rv -lt 0 -or $rv -gt 100){return -1}; $rv }
function lighterUnsafe($t,$r){ $x=lighter $t $r; if($x -lt 0){100.0}else{$x} }
function darkerUnsafe($t,$r){ $x=darker $t $r; if($x -lt 0){0.0}else{$x} }
function prefLight($t){ [math]::Round($t) -lt 60.0 }
function fgTone($bg,$ratio){
  $lt=lighterUnsafe $bg $ratio;$dt=darkerUnsafe $bg $ratio;$lr=ratioOfTones $lt $bg;$dr=ratioOfTones $dt $bg
  if(prefLight $bg){ $neg=([math]::Abs($lr-$dr)-lt 0.1)-and($lr -lt $ratio)-and($dr -lt $ratio); if(($lr -ge $ratio)-or($lr -ge $dr)-or $neg){$lt}else{$dt} }
  else{ if(($dr -ge $ratio)-or($dr -ge $lr)){$dt}else{$lt} }
}
function clampD($lo,$hi,$v){ [math]::Max($lo,[math]::Min($hi,$v)) }
function ccget($c,$cl){ $low,$n,$m,$h=$c; if($cl -le -1){$low}elseif($cl -lt 0){$n}elseif($cl -lt 0.5){$n+($m-$n)*($cl/0.5)}elseif($cl -lt 1.0){$m+($h-$m)*(($cl-0.5)/0.5)}else{$h} }

function surfTone($role,$isDark,$cl){
  switch($role){
    'surface'{ if($isDark){6}else{98};break}
    'surfaceDim'{ if($isDark){6}else{ccget @(87,87,80,75) $cl};break}
    'surfaceBright'{ if($isDark){ccget @(24,24,29,34) $cl}else{98};break}
    'surfaceContainerLowest'{ if($isDark){ccget @(4,4,2,0) $cl}else{100};break}
    'surfaceContainerLow'{ if($isDark){ccget @(10,10,11,12) $cl}else{ccget @(96,96,96,95) $cl};break}
    'surfaceContainer'{ if($isDark){ccget @(12,12,16,20) $cl}else{ccget @(94,94,92,90) $cl};break}
    'surfaceContainerHigh'{ if($isDark){ccget @(17,17,21,25) $cl}else{ccget @(92,92,88,85) $cl};break}
    'surfaceContainerHighest'{ if($isDark){ccget @(22,22,26,30) $cl}else{ccget @(90,90,84,80) $cl};break}
    'surfaceVariant'{ if($isDark){30}else{90};break}
    'inverseSurface'{ if($isDark){90}else{20};break}
    default{ $null }
  }
}
function hiSurf($isDark,$cl){ if($isDark){surfTone 'surfaceBright' $true $cl}else{surfTone 'surfaceDim' $false $cl} }

$global:R=@{}
function def($n,$l,$d,$bg,$cc,$isBg,$pair){ $global:R[$n]=@{l=$l;d=$d;bg=$bg;cc=$cc;isBg=$isBg;pair=$pair} }
foreach($fam in @('primary','secondary','tertiary')){
  $Cn=$fam.Substring(0,1).ToUpper()+$fam.Substring(1)
  def $fam 40 80 'highest' @(3,4.5,7,7) $true @{a="${fam}Container";b=$fam;delta=10;pol='nearer';together=$false}
  def "${fam}Container" 90 30 'highest' @(1,1,3,4.5) $true @{a="${fam}Container";b=$fam;delta=10;pol='nearer';together=$false}
  def "on$Cn" 100 20 $fam @(4.5,7,11,21) $false $null
  def "on${Cn}Container" 30 90 "${fam}Container" @(3,4.5,7,11) $false $null
  def "${fam}Fixed" 90 90 'highest' @(1,1,3,4.5) $true @{a="${fam}Fixed";b="${fam}FixedDim";delta=10;pol='lighter';together=$true}
  def "${fam}FixedDim" 80 80 'highest' @(1,1,3,4.5) $true @{a="${fam}Fixed";b="${fam}FixedDim";delta=10;pol='lighter';together=$true}
  def "on${Cn}Fixed" 10 10 "${fam}FixedDim" @(4.5,7,11,21) $false $null
  def "on${Cn}FixedVariant" 30 30 "${fam}FixedDim" @(3,4.5,7,11) $false $null
}
def 'inversePrimary' 80 40 'inverseSurface' @(3,4.5,7,7) $false $null
def 'onSurface' 10 90 'highest' @(4.5,7,11,21) $false $null
def 'onSurfaceVariant' 30 80 'highest' @(3,4.5,7,11) $false $null
def 'inverseOnSurface' 95 20 'inverseSurface' @(4.5,7,11,21) $false $null

function baseTone($n,$isDark){ if($isDark){$global:R[$n].d}else{$global:R[$n].l} }
function bgOf($bg,$isDark,$cl){ if($bg -eq 'highest'){return hiSurf $isDark $cl}; $st=surfTone $bg $isDark $cl; if($null -ne $st){return $st}; return resolve $bg $isDark $cl }

function resolve($n,$isDark,$cl){
  $st=surfTone $n $isDark $cl; if($null -ne $st){return $st}
  $r=$global:R[$n]
  if($null -ne $r.pair){
    $p=$r.pair; $expDir= if($isDark){1}else{-1}
    $aNear = ($p.pol -eq 'nearer') -or ($p.pol -eq 'lighter' -and -not $isDark) -or ($p.pol -eq 'darker' -and $isDark)
    $near = if($aNear){$p.a}else{$p.b}; $far= if($aNear){$p.b}else{$p.a}
    $nTone=baseTone $near $isDark; $fTone=baseTone $far $isDark
    $bt=bgOf $r.bg $isDark $cl
    $nC=ccget $global:R[$near].cc $cl; $fC=ccget $global:R[$far].cc $cl
    if((ratioOfTones $bt $nTone) -lt $nC){$nTone=fgTone $bt $nC}
    if((ratioOfTones $bt $fTone) -lt $fC){$fTone=fgTone $bt $fC}
    if((($fTone-$nTone)*$expDir) -lt $p.delta){
      $fTone=clampD 0 100 ($nTone+$p.delta*$expDir)
      if((($fTone-$nTone)*$expDir) -ge $p.delta){}else{ $nTone=clampD 0 100 ($fTone-$p.delta*$expDir) }
    }
    if((50 -le $nTone)-and($nTone -lt 60)){
      if($expDir -gt 0){$nTone=60;$fTone=[math]::Max($fTone,$nTone+$p.delta*$expDir)}else{$nTone=49;$fTone=[math]::Min($fTone,$nTone+$p.delta*$expDir)}
    }elseif((50 -le $fTone)-and($fTone -lt 60)){
      if($p.together){ if($expDir -gt 0){$nTone=60;$fTone=[math]::Max($fTone,$nTone+$p.delta*$expDir)}else{$nTone=49;$fTone=[math]::Min($fTone,$nTone+$p.delta*$expDir)} }
      else{ if($expDir -gt 0){$fTone=60}else{$fTone=49} }
    }
    if($n -eq $near){return $nTone}else{return $fTone}
  }
  $answer=baseTone $n $isDark
  if($null -eq $r.bg){return $answer}
  $bt=bgOf $r.bg $isDark $cl
  $des=ccget $r.cc $cl
  if((ratioOfTones $bt $answer) -lt $des){$answer=fgTone $bt $des}
  if($r.isBg -and (50 -le $answer)-and($answer -lt 60)){ if((ratioOfTones 49 $bt) -ge $des){$answer=49}else{$answer=60} }
  return $answer
}

$order=@('primary','onPrimary','primaryContainer','onPrimaryContainer','inversePrimary','primaryFixed','primaryFixedDim','onPrimaryFixed','onPrimaryFixedVariant','secondary','onSecondary','secondaryContainer','onSecondaryContainer','secondaryFixed','secondaryFixedDim','onSecondaryFixed','onSecondaryFixedVariant','tertiary','onTertiary','tertiaryContainer','onTertiaryContainer','tertiaryFixed','tertiaryFixedDim','onTertiaryFixed','onTertiaryFixedVariant','surface','surfaceVariant','inverseSurface','inverseOnSurface','surfaceDim','surfaceBright','surfaceContainerLowest','surfaceContainerLow','surfaceContainer','surfaceContainerHigh','surfaceContainerHighest','onSurface','onSurfaceVariant')
$lv=@(0.0,0.5,1.0)
"{0,-24} {1,-14} {2,-14}" -f 'role','LIGHT N/M/H','DARK N/M/H'
foreach($n in $order){
  $lt=foreach($cl in $lv){[int][math]::Round((resolve $n $false $cl))}
  $dt=foreach($cl in $lv){[int][math]::Round((resolve $n $true $cl))}
  "{0,-24} {1,-14} {2,-14}" -f $n,($lt -join '/'),($dt -join '/')
}
