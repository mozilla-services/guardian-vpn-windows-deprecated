Set-DefaultAWSRegion -Region us-east-1
# validate cloudformation template
$content = Get-Content -Path .\test\smoke\aws\template.json -Raw
Test-CFNTemplate -TemplateBody $content

$p1 = new-object -Type Amazon.CloudFormation.Model.Parameter -Property @{ParameterKey="KeyName"; ParameterValue="$env:KEY_NAME"}
$p2 = new-object -Type Amazon.CloudFormation.Model.Parameter -Property @{ParameterKey="S3BucketName"; ParameterValue="$env:S3_BUCKET"}
$p3 = new-object -Type Amazon.CloudFormation.Model.Parameter -Property @{ParameterKey="ImageId"; ParameterValue="$env:IMAGE_ID"}
$p4 = new-object -Type Amazon.CloudFormation.Model.Parameter -Property @{ParameterKey="BuildNumber"; ParameterValue="$env:CIRCLE_BUILD_NUM"}
$p5 = new-object -Type Amazon.CloudFormation.Model.Parameter -Property @{ParameterKey="GitBranch"; ParameterValue="$env:CIRCLE_BRANCH"}
$p6 = new-object -Type Amazon.CloudFormation.Model.Parameter -Property @{ParameterKey="GitPrivateKeyPart1"; ParameterValue="$env:GIT_PRIVATE_KEY_1"}
$p7 = new-object -Type Amazon.CloudFormation.Model.Parameter -Property @{ParameterKey="GitPrivateKeyPart2"; ParameterValue="$env:GIT_PRIVATE_KEY_2"}
$p8 = new-object -Type Amazon.CloudFormation.Model.Parameter -Property @{ParameterKey="GitPublicKey"; ParameterValue="$env:GIT_PUBLIC_KEY"}
$p9 = new-object -Type Amazon.CloudFormation.Model.Parameter -Property @{ParameterKey="KeyValue"; ParameterValue="$env:KEY_VALUE"}

$params = @($p1,$p2,$p3,$p4,$p5,$p6,$p7,$p8,$p9)

$stack = "guardian-stack-$env:CIRCLE_BUILD_NUM"
# create stack
New-CFNStack -StackName $stack -TemplateBody $content -Capability CAPABILITY_IAM -Parameter $params

# wait stack to complete
Wait-CFNStack -StackName $stack -Timeout 300