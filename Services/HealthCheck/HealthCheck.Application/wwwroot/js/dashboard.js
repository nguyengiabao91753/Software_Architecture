const HEALTH_JSON = "/health";
const CONSUL_SERVICES = "/consul-services";

// Fetch JSON
async function fetchJson(url) {
    const r = await fetch(url);
    if (!r.ok) throw new Error(`${url} returned ${r.status}`);
    return r.json();
}

// UI NEW DATA
let servicesData = [];
let webhooks = [];
let selectedService = null;
let showSecrets = {};
let formState = { name: '', url: '', events: [], sendTo: '' };

// Load API data → Map → Render UI
async function loadData() {
    await Promise.all([
        loadServices(),
        loadHealthSummary()
    ]);
}

// Load Consul data for Services UI
async function loadServices() {
    const consul = await fetchJson(CONSUL_SERVICES);

    servicesData = consul.map(s => {
        const node = s.nodes?.[0];
        const check = node?.checks?.[0];

        return {
            id: s.service,
            name: s.service,
            status: check?.status ?? 'Unknown',
            duration: check?.output ?? '',
            port: node?.port ?? '',
            endpoint: `${node?.address}:${node?.port}`,
            tags: s.tags ?? []
        };
    });

    renderServices();
    const stats = buildCategoryStats();
    renderCategoryChart(stats);
}

// Load Health Summary for Status Chart
async function loadHealthSummary() {
    const health = await fetchJson(HEALTH_JSON);
    const entries = Object.values(health.entries || {});

    const summary = {
        healthy: entries.filter(x => x.status === "Healthy").length,
        degraded: entries.filter(x => x.status === "Degraded").length,
        unhealthy: entries.filter(x => x.status === "Unhealthy").length
    };

    renderHealthChart(summary);
}


// ========================================
// RENDER UI (UI MỚI NGUYÊN BẢN)
// ========================================

document.addEventListener('DOMContentLoaded', () => {
    loadData();          // <-- Replace mock -> load API
    renderWebhooks();    // Keep UI, waiting for API later
    setupTabs();
    setupWebhookForm();
});

// --- Services List (UI mới giữ nguyên) ---
function renderServices() {
    const list = document.getElementById("servicesList");

    list.innerHTML = servicesData.map(s => `
        <div class="service-item ${selectedService === s.id ? 'expanded' : ''}" data-service-id="${s.id}">
            <div class="service-header">
                <svg class="service-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <rect x="2" y="2" width="20" height="8"></rect>
                    <rect x="2" y="14" width="20" height="8"></rect>
                </svg>
                <div class="service-main">
                    <div class="service-name">
                        ${s.name}
                        <span class="status-badge healthy">${s.status}</span>
                    </div>
                    <div class="service-details">
                        <span>${s.endpoint}</span>
                        <span>Port: ${s.port}</span>
                        <span>${s.duration}</span>
                    </div>
                    <div class="service-tags">
                        ${s.tags.map(t => `<span class="tag">${t}</span>`).join('')}
                    </div>
                </div>
            </div>

            ${selectedService === s.id ? `
                <div class="service-expanded-content">
                    <div class="expanded-grid">
                        <div>
                            <span class="expanded-label">Response Time:</span>
                            <p class="expanded-value">${s.duration}</p>
                        </div>
                        <div>
                            <span class="expanded-label">Service ID:</span>
                            <p class="expanded-value">${s.id}</p>
                        </div>
                    </div>
                </div>
            ` : ''}
        </div>
    `).join('');

    // Expand handler
    list.querySelectorAll('.service-item').forEach(item => {
        item.addEventListener('click', () => {
            const id = item.getAttribute('data-service-id');
            selectedService = selectedService === id ? null : id;
            renderServices();
        });
    });
}


// --- Health Chart (UI mới) ---
let chartInstance = null;

function renderHealthChart(summary) {
    const ctx = document.getElementById("healthChart").getContext("2d");

    if (chartInstance) chartInstance.destroy();

    chartInstance = new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: ["Healthy", "Degraded", "Unhealthy"],
            datasets: [{
                data: [summary.healthy, summary.degraded, summary.unhealthy],
                backgroundColor: ["#28a745", "#ffc107", "#dc3545"]
            }]
        },
        options: {
            responsive: false,   // <-- Disable auto resize
            maintainAspectRatio: false,
            plugins: {
                legend: { position: "bottom" }
            }
        }
    });
}

// ===============================
// CATEGORY CHART (Bar UI)
// ===============================

// Rule phân loại theo tên service
function detectCategory(serviceName) {
    const name = serviceName.toLowerCase();

    if (name.endsWith("-api")) return "API Services";
    if (name.includes("consul")) return "Service Discovery";

    return "Other";
}

// Phân tích servicesData → thống kê theo category
function buildCategoryStats() {
    const stats = {};

    servicesData.forEach(svc => {
        const category = detectCategory(svc.name);
        stats[category] = (stats[category] || 0) + 1;
    });

    return stats;
}

// Render Bar Chart UI
function renderCategoryChart(stats) {
    const container = document.querySelector(".bar-chart");
    container.innerHTML = "";

    const total = Object.values(stats).reduce((a, b) => a + b, 0);

    Object.entries(stats).forEach(([name, count]) => {
        const percent = total === 0 ? 0 : (count / total) * 100;

        const bar = document.createElement("div");
        bar.className = "bar-item";
        bar.innerHTML = `
            <div class="bar-label">${name}</div>
            <div class="bar-wrapper">
                <div class="bar" style="width:${percent}%"></div>
            </div>
            <div class="bar-value">${count}</div>
        `;

        container.appendChild(bar);
    });
}


document.addEventListener("DOMContentLoaded", () => {
    const tabButtons = document.querySelectorAll(".tab-button");
    const tabContents = document.querySelectorAll(".tab-content");

    tabButtons.forEach(button => {
        button.addEventListener("click", () => {
            const target = button.getAttribute("data-tab");

            // Remove active from buttons
            tabButtons.forEach(btn => btn.classList.remove("active"));

            // Hide all tab content
            tabContents.forEach(content => content.classList.remove("active"));

            // Add active to clicked button
            button.classList.add("active");

            // Show target tab
            const targetTab = document.getElementById(target);
            if (targetTab) {
                targetTab.classList.add("active");
            }
        });
    });
});

