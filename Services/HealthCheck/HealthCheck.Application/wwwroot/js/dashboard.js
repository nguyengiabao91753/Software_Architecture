const HEALTH_JSON = "/health";
const CONSUL_SERVICES = "/consul-services";

async function fetchJson(url) {
    const r = await fetch(url);
    if (!r.ok) throw new Error(`${url} returned ${r.status}`);
    return r.json();
}

function mapStatusColor(status) {
    if (!status) return 'secondary';
    status = status.toLowerCase();
    if (status.includes('passing') || status.includes('healthy') || status.includes('ok')) return 'success';
    if (status.includes('warning')) return 'warning';
    if (status.includes('critical') || status.includes('down') || status.includes('unhealthy')) return 'danger';
    return 'secondary';
}

function createCard(title, value, icon = '') {
    return `
    <div class="col-md-3 mb-2">
      <div class="card shadow-sm">
        <div class="card-body">
          <h6 class="card-title">${title}</h6>
          <h3 class="card-text">${value}</h3>
        </div>
      </div>
    </div>`;
}

let statusChart = null;

async function refreshDashboard() {
    try {
        // health-json contains aggregated health checks (from HealthChecks UI)
        const health = await fetchJson(HEALTH_JSON);
        // consul raw
        const consul = await fetchJson(CONSUL_SERVICES);

        // Summary cards
        const totalChecks = (health.status ? 1 : 0) + (health.entries ? Object.keys(health.entries).length : 0);
        const passing = Object.values(health.entries || {}).filter(e => e.status === "Healthy").length;
        const degraded = Object.values(health.entries || {}).filter(e => e.status === "Degraded").length;
        const unhealthy = Object.values(health.entries || {}).filter(e => e.status === "Unhealthy").length;

        document.getElementById('summary-cards').innerHTML = [
            createCard('Total Checks', totalChecks),
            createCard('Passing', passing),
            createCard('Degraded', degraded),
            createCard('Unhealthy', unhealthy)
        ].join('');

        // Quick stats
        const quickStats = document.getElementById('quick-stats');
        quickStats.innerHTML = `
            <li class="list-group-item">Services registered: <strong>${consul.length}</strong></li>
            <li class="list-group-item">Checks passing: <strong>${passing}</strong></li>
            <li class="list-group-item">Checks degraded: <strong>${degraded}</strong></li>
            <li class="list-group-item">Checks unhealthy: <strong>${unhealthy}</strong></li>
        `;

        // Chart: counts by status
        const counts = {
            healthy: passing,
            degraded: degraded,
            unhealthy: unhealthy
        };

        const ctx = document.getElementById('statusChart').getContext('2d');
        if (statusChart) statusChart.destroy();
        statusChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Healthy', 'Degraded', 'Unhealthy'],
                datasets: [{
                    data: [counts.healthy, counts.degraded, counts.unhealthy],
                    backgroundColor: ['#28a745', '#ffc107', '#dc3545']
                }]
            },
            options: { plugins: { legend: { position: 'bottom' } } }
        });

        // Fill services table
        const tbody = document.querySelector('#services-table tbody');
        tbody.innerHTML = '';
        for (const s of consul) {
            for (const node of s.nodes) {
                for (const c of node.checks) {
                    const cls = mapStatusColor(c.status);
                    const row = document.createElement('tr');
                    row.className = `table-${cls} table-sm`;
                    row.innerHTML = `
                        <td>${s.service}</td>
                        <td>${node.id}</td>
                        <td>${node.address}</td>
                        <td>${node.port}</td>
                        <td>${c.status}</td>
                        <td><pre style="white-space: pre-wrap; margin:0;">${c.output ?? ''}</pre></td>
                    `;
                    tbody.appendChild(row);
                }
            }
        }
    } catch (err) {
        console.error("Dashboard refresh error:", err);
    }
}

// initial load
refreshDashboard();
// auto refresh every 10s
setInterval(refreshDashboard, 100000);
