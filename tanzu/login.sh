export KUBECTL_VSPHERE_PASSWORD='VMware1!'
kubectl vsphere login --insecure-skip-tls-verify --server 10.0.10.11 --tanzu-kubernetes-cluster-namespace ordernow -u administrator@vsphere.local
kubectl config set-context ordernow --namespace=ordernow
